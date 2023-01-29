
#include <io.h>
#include "lcd.c"

    #define S_OFF 0x06  
    #define S_VD1 0 
    #define S_VD2 1
    #define S_VD3 2
    #define S_CD1 3
    #define S_CD2 4
    #define S_CD3 5
    #define S_VS1 6
    #define S_VS2 7
    #define S_VS3 8
    #define S_CS1 9
    #define S_CS2 10   
    #define S_CS3 11
    #define S_SCT 12 
    #define S_OTT 13

flash unsigned int temptable[15]=
{
348 ,
429 ,
505 ,
575 ,
640 ,
699 ,
754 ,
802 ,
845 ,
883 ,
915 ,
942 ,
963 ,
978 ,
988 ,


} ;
unsigned char settings=0;
unsigned char flag=0;
unsigned int mah=0;
signed short voltageSET=0;
signed short currentSET=0;
//signed short adj=0;
signed short menu=0;
signed short modeswitch=0;
unsigned char mode=0;
unsigned char vchanged=0;
unsigned char cchanged=0;
 short* tempvar2=0;
interrupt [TIM0_OVF] void timer0_ovf_isr(void)
{
TCNT0=0x06;
flag++;
}





unsigned char EERead(unsigned char adr)
{
while(EECR&0x02);
EEARH=0;
EEARL=adr;
EECR|=(1<<EERE);
return EEDR;
}
void EEWrite(unsigned char adr,unsigned char data)
{
#asm("cli")
while(EECR&0x02);
EEARH=0;
EEARL=adr;
EEDR=data;
EECR&=~(1<<EEWE);
EECR|=(1<<EEMWE);
EECR|=(1<<EEWE);

}
void Ewint(unsigned char adr,unsigned int l)
{
EEWrite(adr,l>>8);
EEWrite(adr+1,l);
}
void Save()
{
#asm("cli")
//EEWrite(0,voltageSET>>8);
//EEWrite(1,voltageSET);
//EEWrite(2,currentSET>>8);
//EEWrite(3,currentSET);
Ewint(0,voltageSET);
Ewint(2,currentSET);
Ewint(4,mah);
}


unsigned int Erint(unsigned char adr)
{
return (EERead(adr)<<8)+EERead(adr+1);
}


interrupt [ANA_COMP] void ana_comp_isr(void)
{
Save();
while(1);
}
void TimerInit()
{
   // DDRD|=0x01;
    TCCR0=(0<<CS02) | (1<<CS01) | (1<<CS00);
    TCNT0=0x06;
    TIMSK=(0<<OCIE2) | (0<<TOIE2) | (0<<TICIE1) | (0<<OCIE1A) | (0<<OCIE1B) | (0<<TOIE1) | (1<<TOIE0);   
    DDRB|=0x06;
    TCCR1A=(1<<COM1A1) | (0<<COM1A0) | (1<<COM1B1) | (0<<COM1B0) | (1<<WGM11) | (0<<WGM10);
    TCCR1B=(0<<ICNC1) | (0<<ICES1) | (1<<WGM13) | (1<<WGM12) | (0<<CS12) | (0<<CS11) | (1<<CS10);
    TCNT1H=0x00;
    TCNT1L=0x00;
    ICR1H=0x13;
    ICR1L=0x88;
    OCR1AH=0x00;
    OCR1AL=0x00;
    OCR1BH=0x00;
    OCR1BL=0x00;
    
    ACSR=(0<<ACD) | (0<<ACBG) | (0<<ACO) | (1<<ACI) | (1<<ACIE) | (0<<ACIC) | (1<<ACIS1) | (0<<ACIS0);
SFIOR=(0<<ACME);
}

void I2CInit()
{
 TWBR=64;
 TWSR&=~0x03;
 TWCR=0x44;
}
void SendStop()
{
TWCR|=(1<<TWSTO); 
TWCR|=(1<<TWINT); 
}
unsigned char I2CSend(unsigned char adr,char* data,unsigned char cnt)
{
unsigned char i=0;
 TWCR|=(1<<TWEA);               
 TWCR|=(1<<TWSTA);
 while(!(TWCR&0x80));
 if ((TWSR&0xF8)!=0x08) {SendStop(); return 1; } 
 TWDR=adr<<1; 
 TWCR&=~(1<<TWSTA);
 TWCR|=(1<<TWINT); 
 while(!(TWCR&0x80));  
 if ((TWSR&0xF8)!=0x18) {SendStop(); return 2; }
 for (i=0;i<cnt;i++) { 
    TWDR=data[i]; 
    TWCR|=(1<<TWINT); 
    while(!(TWCR&0x80)); 

    if ((TWSR&0xF8)!=0x28){SendStop(); return 3; }
  }
  SendStop(); 
  return 0;
}
unsigned char I2CRead(unsigned char adr,char* data,unsigned char cnt)
{
unsigned char i=0;
 TWCR|=(1<<TWEA);
 TWCR|=(1<<TWSTA);
 while(!(TWCR&0x80));
 if ((TWSR&0xF8)!=0x08){SendStop(); return 1; }
 TWDR=(adr<<1)|0x01; 
 TWCR&=~(1<<TWSTA);
 TWCR|=(1<<TWINT); 
 while(!(TWCR&0x80));
 if ((TWSR&0xF8)!=0x40) {SendStop();return 2; } 
 for (i=0;i<cnt;i++) {
    if (i==cnt-1) TWCR&=~(1<<TWEA);
    TWCR|=(1<<TWINT);    
    while(!(TWCR&0x80));
    if ((TWSR&0xF8)!=0x50&&(TWSR&0xF8)!=0x58 ) {SendStop();return 3;}
    data[i]=TWDR; 
    
  }
   SendStop();
   return 0;
}

void ADS1115Select(unsigned char ch)
{
    unsigned char t[3]; 
    t[0]=0x01;
    t[1]=0x04|(ch<<4);
    t[2]=0x83;
    I2CSend(0x48,t,3);      
    t[0]=0;
    I2CSend(0x48,t,1);
}

unsigned short abs(signed short a)
{
  signed short b=a; 
  if (b<0) b=0-b;  
  return b;
}

unsigned short ADS1115Read()
{
    
    signed short raw=0;     
    unsigned char adc[2];
    I2CRead(0x48,adc,2);
    raw=((adc[0]<<8)+adc[1]);             
    //return abs(raw);
    return raw;
}

void ADCInit()
{
  ADMUX=2;
  ADCSRA=(1<<ADEN) | (1<<ADSC) | (1<<ADFR) | (1<<ADPS2) | (1<<ADPS1)| (1<<ADPS0) ;

}

void Angle(signed short current,signed short* prev, signed long* out)
{
	signed short dif1=abs(current-*prev);
	signed short dif2=abs(current+4096-*prev);
	signed short dif3=abs(current-4096-*prev);
	if (dif1<dif2 && dif1<dif3)
	{
		(*out)+=(long)current-*prev;
	}
	else if (dif2<dif1 && dif2<dif3)
	{
		(*out)+=(long)current+4096-*prev;
	}
	else if (dif3<dif2 && dif3<dif1)
	{
		(*out)+=(long)current-4096-*prev;
	}
}
/*
void Angle2(signed short current,signed short* prev,signed long* der, signed long* out)
{
    signed short dif1;
    signed short dif2;
    signed short dif3;
    if (*der>4096) *der=4096;  
    else if (*der<-4096) *der=-4096; 
    dif1=abs((long)(current-*prev)-*der);
	dif2=abs((long)(current-*prev+4096)-*der);
	dif3=abs((long)(current-*prev-4096)-*der);
	if (dif1<dif2 && dif1<dif3)
	{    
        *der=(long)current-*prev;
		//(*out)+=*der;
        
	}
	else if (dif2<dif1 && dif2<dif3)
	{   
        *der=(long)current+4096-*prev;
		//(*out)+=
	}
	else if (dif3<dif2 && dif3<dif1)
	{   
        *der=(long)current-4096-*prev;
		//(*out)+=
	}
  
 (*out)+=*der;
}*/
unsigned short Filter(unsigned short new,signed short* mem,signed long* sum)
 {  
    //signed long res=0; 
    unsigned char i=0;
    i= mem[0];
    *sum-=(long)mem[i]; 
    mem[i]=new;
    *sum+=(long)mem[i];
    i++;
    if(i>4) i=1; 
    mem[0]=i;
    return (*sum)/4;
  }
  
void ReadHandle(signed short* prev,signed long* abspos,unsigned char handle)
{
    unsigned char t[1];
    unsigned char regbuf[2];
    signed short vregcp=0; 
    if (handle==0) 
    {
        DDRD.0=0; 
        DDRD.1=1; 
    }
    else 
    {
        DDRD.1=0;
        DDRD.0=1;
    }    
    t[0]=0x0C; 
    I2CSend(0x36,t,1); 
    if(I2CRead(0x36,regbuf,2)) return;  
        DDRD.0=1; 
        DDRD.1=1;        
    vregcp=((regbuf[0]<<8)+regbuf[1]);
    if(*prev!=10000) 
    {           
    Angle(vregcp,prev,abspos);
    }        
    *prev=vregcp;
    
}
void Words()
{

ClearBuf();
PrintString("voltage",8,2);              
SendBuf(0,0);
ClearBuf();
PrintString("current",8,2);              
SendBuf(1,0);
ClearBuf();
PrintString("power",8,2);              
SendBuf(0,32);
ClearBuf();
PrintString("energy",8,2);              
SendBuf(1,32);
}
void ReadHandle2(signed long* abspos,unsigned char handle)
{
  signed int dir;
  //signed int t;  
/*
  if(*abspos>100) {EncoderStep(1,handle);*abspos=0;  }
  if(*abspos<-100) {EncoderStep(-1,handle);*abspos=0;  }
  */
  if (*abspos/128!=0)
  { 
      
    dir=*abspos/128*10;
    
      if (mode==0)
        {
            if (handle==0) {
                if (voltageSET>9999) dir=dir/2*10;
                voltageSET+=dir;
                if(voltageSET<0)voltageSET=0;
                if(voltageSET>30000)voltageSET=30000;
                vchanged=20;
            }
            else {
                if (currentSET>9999) dir=dir/2*10;
                currentSET+=dir;
                if(currentSET<0){modeswitch+=dir;currentSET=0;} 
                if (currentSET>100)modeswitch=0;
                if(modeswitch<-1000){
                    mode=1;
                    modeswitch=0;
                    DisplayFullUpdate();
                    }
                if(currentSET>20000)currentSET=20000;
                cchanged=20;
            }
        } 
        else if(mode==1)
        {   
            vchanged=0;
            cchanged=0;
            if (handle==0) {
                *tempvar2+=*abspos/128;
                //if(*tempvar2<0)adj=0;
                //if(*tempvar2>30000)adj=30000; 
            }
            else {
                menu+=dir;  
                if(menu<0){modeswitch+=dir;menu=0;} 
                if(modeswitch<-1000){mode=0;mah=0;modeswitch=0;settings=1;}
            }
        
        }
      *abspos=*abspos%128;
  }
}
  
void ReadESettings(signed int* sett)
{
unsigned char i=0;
for(i=0;i<14;i++)
{
 sett[i]=Erint(S_OFF+i*2);
}
/*
 vdispf1=Erint(6);
    vdispf2=Erint(8);
    vdispb=Erint(10);
    cdispf1=Erint(12);
    cdispf2=Erint(14);
    cdispb=Erint(16);
    vsetf1=Erint(18);
    vsetf2=Erint(20); 
    vsetb=Erint(22);
    csetf1=Erint(24);
    csetf2=Erint(26);
    csetb=Erint(28);
    */
}
void WriteESettings(signed int* sett)
{
unsigned char i=0;
#asm("cli")
for(i=0;i<14;i++)
{

 Ewint(S_OFF+i*2,sett[i]);
}
#asm("sei")
}

unsigned char ConvertTemp(unsigned int adc)
{
 unsigned char i=0; 
 unsigned char res=0;  
 unsigned int diff=0;
 while(i<14&&temptable[i+1]<adc) i++;
 if (i==14) return 140; 
 diff= temptable[i+1]-temptable[i];

 res=i*10+(adc-temptable[i])*10/diff;
  return res;
}


void main(void)
{
    unsigned long mas=0;
    unsigned long millisec=0;
    signed int tempvar=0;    
    
    unsigned char protection=0;
    
    signed short voltage=0;    
    signed short current=0;     
    unsigned long power=0;  
    
    signed short VoltFilt[5];
    signed long VSum=0;  
    signed short AmpFilt[5];
    signed long ASum=0; 
    //char str[4];
    /*
    unsigned int vdispf1=995;
    unsigned int vdispf2=1000;
    signed int vdispb=0;
    unsigned int cdispf1=5555;
    unsigned int cdispf2=8000;
    signed int cdispb=0;
    unsigned int vsetf1=100;
    unsigned int vsetf2=600; 
    signed int vsetb=0;
    unsigned int csetf1=1000;
    unsigned int csetf2=5555;
    signed int csetb=0;
     */ 
     /*
    unsigned int vdispf1;
    unsigned int vdispf2;
    signed int vdispb;
    unsigned int cdispf1;
    unsigned int cdispf2;
    signed int cdispb;
    unsigned int vsetf1;;
    unsigned int vsetf2;
    signed int vsetb;
    unsigned int csetf1;
    unsigned int csetf2;
    signed int csetb; 
     */
    signed int setting[14];
     
     
    unsigned short a=0;     
    signed long vreg=0; 
    signed long creg=0;    
    signed short vregpp=10000;
    signed short cregpp=10000;
    VoltFilt[0]=1;
    AmpFilt[0]=1; 
    
    PORTD&=~0x03;
    DDRD|=0x03;
              
    delay_ms2(200);
    
    St7920Init();
    I2CInit();
    ADCInit(); 
    
    voltageSET=Erint(0); 
    currentSET=Erint(2);
    mah=Erint(4);
    
    ReadESettings(setting);
    
    
    TimerInit(); 
    ADS1115Select(4);
    Words();
     
    #asm("sei")    
       //while(1);
while (1)
    {  
        a++; 
        
        if (flag>0)
        {  
            millisec+=flag;
            flag=0;
        }
               
        if (mas>2700000)  
            {
            mas-=2700000; 
            mah++;
            if (mah>30000) mah=0;
            } 
            
        //if(a%256==0)
        //{   
            if(!cchanged)
            {
             ReadHandle(&vregpp,&vreg,0);
             ReadHandle2(&vreg,0);
             }
            if (!vchanged)
            {
             ReadHandle(&cregpp,&creg,1);
             ReadHandle2(&creg,1);
            }
        
        //}
        if(mode==0)
        {
        if (a==3)  
        {    
            
        
            ADS1115Select(3);
            voltage=Filter(ADS1115Read(),VoltFilt,&VSum);
             //voltage=ADS1115Read();
            //voltage=(long)voltage*vdispf1/vdispf2+vdispb; 
            voltage=(long)(voltage+setting[S_VD3])*setting[S_VD1]/setting[S_VD2];
            if(voltage<0)voltage=0;
            if(vchanged){
               PrintValue(8,0,voltageSET/1000,voltageSET%1000,'v');
               //PrintValue(8,0,vreg/1000,vreg%1000,'v');
               vchanged--;
            }
            else PrintValue(8,0,voltage/1000,voltage%1000,'v'); //PrintValue(8,0,vreg/1000,vreg%1000,'v');
            SendBuf(0,11); 
            
            //tempvar=(long)voltageSET*vsetf1/vsetf2+vsetb;
            tempvar=(long)(voltageSET+setting[S_VS3])*setting[S_VS1]/setting[S_VS2];
            if(tempvar<0)tempvar=0;  
            if (setting[S_SCT]>0)
            {
                if (voltage<voltageSET*(long)setting[S_SCT]/100) protection|=0x01;
                else protection&=~0x01;
            }
            else protection&=~0x01;
            OCR1AH=tempvar>>8;
            OCR1AL=tempvar;
            
                                  
            //measurement[0]= (long)measurement[0]*995/1000; 
        }
        
        if(a==4)    
        {              
            ADS1115Select(0);
            //current=ADS1115Read();
            //current=4321; 
            current=Filter(ADS1115Read(),AmpFilt,&ASum);
            //current=(long)current*cdispf1/cdispf2+cdispb; 
            current=(long)(current+setting[S_CD3])*setting[S_CD1]/setting[S_CD2];
            if (current<0)current=0;
            
            
            mas+= current*millisec;
            millisec=0;                         
            if(cchanged){
                PrintValue(8,0,currentSET/1000,currentSET%1000,'a',);  
                cchanged--;
            }
            else PrintValue(8,0,current/1000,current%1000,'a',);            
            SendBuf(1,11);
            //tempvar=(long)currentSET*csetf1/csetf2+csetb;   
            
            tempvar=(long)(currentSET+setting[S_CS3])*setting[S_CS1]/setting[S_CS2];
            if(tempvar<0)tempvar=0; 
            if (protection&&tempvar>10) tempvar=10;   
            OCR1BH=tempvar>>8;
            OCR1BL=tempvar;
        }
        if(a==5) 
        {  
            power=(long)voltage*current/1000;
            PrintValue(8,0,power/1000,power%1000,'w');
            SendBuf(0,43);
            ClearBuf();
            //PrintValue(8,0,mah/1000,mah%1000,'h');
            PrintInt(mah,8,0," mah"); 
            SendBuf(1,43);  
            ClearBuf();    
            tempvar=ConvertTemp(ADCL|((unsigned short)ADCH<<8));
            if (setting[S_OTT]>0 && tempvar>setting[S_OTT]) protection|=0x02;
            else protection&=~0x02;
            PrintInt(tempvar,8,0," c"); 
            SendBuf(1,53); 
            
        }
        }
        else if(mode==1)
        {
        //mah=0;
        
        if(menu/256!=tempvar) tempvar=1;
        else tempvar=0;  
        ClearBuf();
        
        switch(menu/256)
        {
          case 0: 
            //str="vd1";
            PrintString("vd1",8,2);
            //tempvar2=&vdispf1;              
            //SendBuf(0,0);
          break;
          case 1: 
          //str="vd2";
            PrintString("vd2",8,2);              
            //SendBuf(0,0);
            //tempvar2=&vdispf2;
          break;
          case 2:
          //str="vd3";
            PrintString("vd3",8,2);              
            //SendBuf(0,0);
            //tempvar2=&vdispb;
          break;
          case 3:
          //str="cd1";
            PrintString("cd1",8,2);              
            //SendBuf(0,0); 
            //tempvar2=&cdispf1;
          break;
          case 4: 
          //str="cd2";
            PrintString("cd2",8,2);              
            //SendBuf(0,0);
            //tempvar2=&cdispf2;
          break;
          case 5: 
          //str="cd3";
            PrintString("cd3",8,2);              
            //SendBuf(0,0);
            //tempvar2=&cdispb;
          break;
          case 6:  
          //str="vs1";
            PrintString("vs1",8,2);              
            //SendBuf(0,0); 
            //tempvar2=&vsetf1;
          break;
          case 7:
          //str="vs2";
            PrintString("vs2",8,2);              
            //SendBuf(0,0);
            //tempvar2=&vsetf2;
          break;
          case 8: 
          //str="vs3";
            PrintString("vs3",8,2);              
            //SendBuf(0,0);
            //tempvar2=&vsetb;
          break;
          case 9:
          //str="cs1";
            PrintString("cs1",8,2);              
            //SendBuf(0,0);
            //tempvar2=&csetf1;
          break;
          case 10: 
          //str="cs2";
            PrintString("cs2",8,2);              
            //SendBuf(0,0);
            //tempvar2=&csetf2;
          break;
          case 11:
          //str="cs3";
            PrintString("cs3",8,2);              
           // tempvar2=&csetb;   
           break;
           case 12:
          //str="cs3";
            PrintString("sct",8,2);              
           // tempvar2=&csetb;  
           break;
           case 13:
          //str="cs3";
            PrintString("ott",8,2);              
           // tempvar2=&csetb;
          break;
          default:
          break;
        }
        
        //PrintString(str,8,2);
        tempvar2=&setting[menu/256];
        
        
        SendBuf(0,0);  
        ClearBuf();
        //PrintValue(8,0,(*tempvar2&0x7FFF)/1000,(*tempvar2&0x7FFF)%1000,'v'); 
        PrintInt(*tempvar2,8,2,""); 
        SendBuf(1,0);
        tempvar=menu/256;
        
        } 
        /*
        if(a==4)
        {
           PrintString("voltage",8);              
            SendBuf(0,0);
        
        }
        if(a==5)
        { 
        PrintString("current",8);              
            SendBuf(1,0);
        }
        if(a==6)
        { 
          PrintString("power",8);              
            SendBuf(0,32);
        }
        */
        if (a==16) 
        {   
            if(settings)
            {   
                PrintString("saving",8,2);              
                SendBuf(0,0);
                #asm("cli") 
                WriteESettings(setting);
                /*
               Ewint(6,vdispf1);
               Ewint(8,vdispf2);
               Ewint(10,vdispb);
               Ewint(12,cdispf1);
               Ewint(14,cdispf2);
               Ewint(16,cdispb);
               Ewint(18,vsetf1);
               Ewint(20,vsetf2);
               Ewint(22,vsetb);
               Ewint(24,csetf1);
               Ewint(26,csetf2);
               Ewint(28,csetb);*/
               settings=0;              
               #asm("sei")
               Words();
            }
            
            
            //PrintString("energy",8);              
            //SendBuf(1,32);
            a=0;
            
        }       
        
          
        
                 
    }
}
