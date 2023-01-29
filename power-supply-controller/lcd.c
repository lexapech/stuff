unsigned char lcdbuf[8][10]  ;

flash unsigned char let[38][8]=
{
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00,
0x00, 0x0E, 0x11, 0x13, 0x15, 0x19, 0x11, 0x0E, 

/* Code: 0x31, ASCII Character: '1' */
0x00, 0x04, 0x0C, 0x04, 0x04, 0x04, 0x04, 0x0E, 

/* Code: 0x32, ASCII Character: '2' */
0x00, 0x0E, 0x11, 0x01, 0x02, 0x04, 0x08, 0x1F, 

/* Code: 0x33, ASCII Character: '3' */
0x00, 0x1F, 0x02, 0x04, 0x02, 0x01, 0x11, 0x0E, 

/* Code: 0x34, ASCII Character: '4' */
0x00, 0x02, 0x06, 0x0A, 0x12, 0x1F, 0x02, 0x02, 

/* Code: 0x35, ASCII Character: '5' */
0x00, 0x1F, 0x10, 0x1E, 0x01, 0x01, 0x11, 0x0E, 

/* Code: 0x36, ASCII Character: '6' */
0x00, 0x06, 0x08, 0x10, 0x1E, 0x11, 0x11, 0x0E, 

/* Code: 0x37, ASCII Character: '7' */
0x00, 0x1F, 0x11, 0x01, 0x02, 0x04, 0x04, 0x04, 

/* Code: 0x38, ASCII Character: '8' */
0x00, 0x0E, 0x11, 0x11, 0x0E, 0x11, 0x11, 0x0E, 

/* Code: 0x39, ASCII Character: '9' */
0x00, 0x0E, 0x11, 0x11, 0x0F, 0x01, 0x02, 0x0C, 

0x00, 0x00, 0x00, 0x0E, 0x01, 0x0F, 0x11, 0x0F, 

/* Code: 0x62, ASCII Character: 'b' */
0x00, 0x10, 0x10, 0x16, 0x19, 0x11, 0x11, 0x1E, 

/* Code: 0x63, ASCII Character: 'c' */
0x00, 0x00, 0x00, 0x0E, 0x10, 0x10, 0x11, 0x0E, 

/* Code: 0x64, ASCII Character: 'd' */
0x00, 0x01, 0x01, 0x0D, 0x13, 0x11, 0x11, 0x0F, 

/* Code: 0x65, ASCII Character: 'e' */
0x00, 0x00, 0x00, 0x0E, 0x11, 0x1F, 0x10, 0x0E, 

/* Code: 0x66, ASCII Character: 'f' */
0x00, 0x06, 0x09, 0x08, 0x1C, 0x08, 0x08, 0x08, 

/* Code: 0x67, ASCII Character: 'g' */
0x00, 0x00, 0x00, 0x0F, 0x11, 0x0F, 0x01, 0x0E, 

/* Code: 0x68, ASCII Character: 'h' */
0x00, 0x10, 0x10, 0x16, 0x19, 0x11, 0x11, 0x11, 

/* Code: 0x69, ASCII Character: 'i' */
0x00, 0x04, 0x00, 0x04, 0x0C, 0x04, 0x04, 0x0E, 

/* Code: 0x6A, ASCII Character: 'j' */
0x00, 0x01, 0x00, 0x03, 0x01, 0x01, 0x09, 0x06, 

/* Code: 0x6B, ASCII Character: 'k' */
0x00, 0x10, 0x10, 0x12, 0x14, 0x18, 0x14, 0x12, 

/* Code: 0x6C, ASCII Character: 'l' */
0x00, 0x0C, 0x04, 0x04, 0x04, 0x04, 0x04, 0x0E, 

/* Code: 0x6D, ASCII Character: 'm' */
0x00, 0x00, 0x00, 0x1A, 0x15, 0x15, 0x15, 0x15, 

/* Code: 0x6E, ASCII Character: 'n' */
0x00, 0x00, 0x00, 0x16, 0x19, 0x11, 0x11, 0x11, 

/* Code: 0x6F, ASCII Character: 'o' */
0x00, 0x00, 0x00, 0x0E, 0x11, 0x11, 0x11, 0x0E, 

/* Code: 0x70, ASCII Character: 'p' */
0x00, 0x00, 0x00, 0x1E, 0x11, 0x1E, 0x10, 0x10, 

/* Code: 0x71, ASCII Character: 'q' */
0x00, 0x00, 0x00, 0x0D, 0x13, 0x0F, 0x01, 0x01, 

/* Code: 0x72, ASCII Character: 'r' */
0x00, 0x00, 0x00, 0x16, 0x19, 0x10, 0x10, 0x10, 

/* Code: 0x73, ASCII Character: 's' */
0x00, 0x00, 0x00, 0x0E, 0x10, 0x0E, 0x01, 0x1E, 

/* Code: 0x74, ASCII Character: 't' */
0x00, 0x08, 0x08, 0x1C, 0x08, 0x08, 0x09, 0x06, 

/* Code: 0x75, ASCII Character: 'u' */
0x00, 0x00, 0x00, 0x11, 0x11, 0x11, 0x13, 0x0D, 

/* Code: 0x76, ASCII Character: 'v' */
0x00, 0x00, 0x00, 0x11, 0x11, 0x11, 0x0A, 0x04, 

/* Code: 0x77, ASCII Character: 'w' */
0x00, 0x00, 0x00, 0x11, 0x11, 0x15, 0x15, 0x0A, 

/* Code: 0x78, ASCII Character: 'x' */
0x00, 0x00, 0x00, 0x11, 0x0A, 0x04, 0x0A, 0x11, 

/* Code: 0x79, ASCII Character: 'y' */
0x00, 0x00, 0x00, 0x11, 0x11, 0x0F, 0x01, 0x0E, 

/* Code: 0x7A, ASCII Character: 'z' */
0x00, 0x00, 0x00, 0x1F, 0x02, 0x04, 0x08, 0x1F, 

 };
flash unsigned char font10x8[10][10]  =
{
0x3C, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 
0x42, 0x3C, 

/* Code: 0x31, ASCII Character: '1' */
0x08, 0x18, 0x28, 0x08, 0x08, 0x08, 0x08, 0x08, 
0x08, 0x7E, 

/* Code: 0x32, ASCII Character: '2' */
0x3C, 0x42, 0x02, 0x02, 0x04, 0x08, 0x10, 0x20, 
0x40, 0x7E, 

/* Code: 0x33, ASCII Character: '3' */
0x3C, 0x42, 0x02, 0x02, 0x0C, 0x02, 0x02, 0x02, 
0x42, 0x3C, 

/* Code: 0x34, ASCII Character: '4' */
0x06, 0x0A, 0x12, 0x22, 0x42, 0x42, 0x7E, 0x02, 
0x02, 0x02, 

/* Code: 0x35, ASCII Character: '5' */
0x7E, 0x40, 0x40, 0x40, 0x7C, 0x02, 0x02, 0x02, 
0x42, 0x3C, 

/* Code: 0x36, ASCII Character: '6' */
0x3C, 0x42, 0x40, 0x40, 0x7C, 0x42, 0x42, 0x42, 
0x42, 0x3C, 

/* Code: 0x37, ASCII Character: '7' */
0x7E, 0x02, 0x02, 0x04, 0x04, 0x08, 0x08, 0x08, 
0x10, 0x10, 

/* Code: 0x38, ASCII Character: '8' */
0x3C, 0x42, 0x42, 0x42, 0x3C, 0x42, 0x42, 0x42, 
0x42, 0x3C, 

/* Code: 0x39, ASCII Character: '9' */
0x3C, 0x42, 0x42, 0x42, 0x3E, 0x02, 0x02, 0x02, 
0x42, 0x3C, 
 } ;
 
void delay_us(unsigned int us)
{
  unsigned int tick = 0;

  while (us--)
  {
    while (tick < 14)
    {
      tick++;
    }
    tick = 0;
  }
}

void delay_ms2(unsigned int ms)
{
  while (ms--)
  {
    delay_us(1000);
  }
}
 
 
 
 
void Send(unsigned char data,unsigned char cmd)
{
       
    PORTC.0=1; 
    SPDR=0b11111000|cmd<<1;
    while(!(SPSR&0x80)); 
    SPDR=data&0xF0;   
    while(!(SPSR&0x80));  
    SPDR=data<<4; 
    while(!(SPSR&0x80));    
    PORTC.0=0;
    delay_us(30); 
  }
  /*   
void Send2(unsigned char data,unsigned char cmd)
{
      
    PORTB.2=1; 
    SPDR=0b11111000|cmd<<1;
    while(!(SPSR&0x80)); 
    SPDR=data&0xF0;   
    while(!(SPSR&0x80));  
    SPDR=data<<4; 
    while(!(SPSR&0x80));    
    PORTB.2=0;
    delay_us(120); 
     
     
    unsigned char i=0;
     PORTC.0=1; 
     
     for(i=0;i<8;i++)
     { 
     PORTB.5=1; 
      if((0b11111000|cmd<<1)&(1<<(7-i))) PORTB.3=1;
      else   PORTB.3=0;  
      //delay_us(1);   
      
      delay_us(1);   
      PORTB.5=0;   
      delay_us(1);
     } 
     
     
     for(i=0;i<8;i++)
     { 
     PORTB.5=1; 
      if((data&0xF0)&(1<<(7-i))) PORTB.3=1;
      else   PORTB.3=0;  
     // delay_us(1);   
      
      delay_us(1);   
      PORTB.5=0;   
      delay_us(1);
     }
     for(i=0;i<8;i++)
     { 
     PORTB.5=1; 
      if((data<<4)&(1<<(7-i))) PORTB.3=1;
      else   PORTB.3=0;  
     // delay_us(1);   
      
      delay_us(1);   
      PORTB.5=0;   
      delay_us(1);
     } 
     PORTC.0=0;
      delay_us(60);   
       
}
*/
void ST7920_Ext_SetGDRAMAddr(unsigned char VertAddr, unsigned char HorizAddr)
{
/*
  unsigned char Data = 0x80;
  Data |= (VertAddr & 0x7F);
  Send(Data,0);
  Data = 0x80;
  Data |= (HorizAddr & 0x0F);
  Send(Data,0);
  */
   // unsigned char Data = 0x80;
  //Data |= (VertAddr & 0x7F);
  Send(VertAddr|0x80,0);
  //Data = 0x80;
  //Data |= (HorizAddr & 0x0F);
  Send(HorizAddr|0x80,0);
  
  //delay_us(72);   
 // delay_us(36);
}


void DisplayFullUpdate()
{
    unsigned char Row = 0;
    unsigned char Col = 0;
    for (Row = 0; Row < 32; Row++)
    {
        ST7920_Ext_SetGDRAMAddr(Row,0);         
        for (Col = 0; Col < 32; Col++) 
        {            
            Send(0,1);  
        }
        
            
    }
      
}
/*
void Dot(unsigned char x,unsigned char y)
{
         unsigned char Col = 0;  
         unsigned short t=0; 
        ST7920_Ext_SetGDRAMAddr(y%32, x/16+(y/32)*8);  
           t=1<<(15-(x%16))  ;       
        Send(t>>8,1);  
        Send(t&0xFF,1); 

}
*/
void SendBuf(unsigned char x,unsigned char y)
{
    unsigned char Row = 0;
    unsigned char Col = 0;
    for (Row = y; Row < y+10; Row++)
    {
        ST7920_Ext_SetGDRAMAddr(Row%32, (x*4)+(y/32)*8);         
        for (Col = 0; Col < 8; Col++) 
        {    
        
            Send(lcdbuf[Col][Row-y],1);  
        }
        
            
    }

}


/*
void Digit8(unsigned char d,unsigned char x,unsigned char y)
{
    unsigned char i=0;
    for (i=0;i<8;i++)
    {
        lcdbuf[x/8][i+y]|=font8x8[d][i]>>(x%8);
        lcdbuf[x/8+1][i+y]|=font8x8[d][i]<<(8-(x%8));
    }

}
 */
void Digit10(unsigned char d,unsigned char x,unsigned char y)
{
    unsigned char i=0;
    for (i=0;i<10;i++)
    {
        lcdbuf[x/8][i+y]|=font10x8[d][i]>>(x%8);
        lcdbuf[x/8+1][i+y]|=font10x8[d][i]<<(8-(x%8));
    }

}
void ClearBuf()
{
 unsigned char i=0;
    for(i=0;i<80;i++) 
       {
           lcdbuf[i/10][i%10] =0;
       } 
}
void Char(char ch,unsigned char x,unsigned char y)
{
unsigned char i=0;
  char d=0;
  if(ch>=0x61) 
  {
     d=ch-0x55;
  }
  else if (ch=='-') d=1; 
  else if (ch==' ') d=0;
  else
  {
    d=ch-0x2E;
  }
    for (i=0;i<8;i++)
    {
        lcdbuf[x/8][i+y]|=(let[d][i]<<3)>>(x%8);
        lcdbuf[x/8+1][i+y]|=(let[d][i]<<3)<<(8-(x%8));
    }
  }
void PrintString(char* str,unsigned char x,unsigned char y)
{
    unsigned char i=0;
    unsigned char pos=x;
    //ClearBuf();
    while(str[i]!=0)
       {  
            Char(str[i],pos,y); 
            pos+=6;
           i++;
       }
}



void IntToStr(signed short val,char* str, char* len)
{
unsigned char sign=0;
unsigned char pos=0;
unsigned char offset=0;
unsigned short div=0;

if (val<0)
{
  str[sign]='-'; 
  div=-val;
  sign++;
}
else
{
div=val;
 }
offset++;
if (div>9) offset++;
if (div>99) offset++;
if (div>999) offset++;
if (div>9999) offset++;

for(pos=offset+sign+1;pos>sign+1;pos--)
{
    str[pos-2]='0'+(div%10);
    div=div/10;
}
*len=offset+sign;
str[*len]=0;
}
void PrintInt(signed short val,unsigned char x,unsigned char y,char* l)
{
    char str[10]; 
    char len=0; 
    unsigned char i=0;
    IntToStr(val,str,&len);
    while(l[i]!=0)
    {  
        str[len+i]=l[i];
        i++;
    } 
    str[len+i]=0;
    PrintString(str,x,y);
}
void FracToStr(unsigned short integer,unsigned short frac,char* str)
{
    unsigned char len=0;
    IntToStr(integer,str,&len);
    str[len]='.';
    str[len+1]='0'+frac/100;
    str[len+2]='0'+frac/10%10;
    str[len+3]='0'+frac%10; 
    str[5]=0;
}
void PrintValue(unsigned char x,unsigned char y,unsigned short round,unsigned short frac,char ch)
{
    char str[6];
    unsigned char i=0; 
    unsigned char pos=x; 
    ClearBuf();
    FracToStr(round,frac,str);
    while(i<5) 
    { 
    if (str[i]=='-') continue;
    if (str[i]=='.')
    {
       lcdbuf[pos/8][8+y]|=192;
        lcdbuf[pos/8][9+y]|=192;
        pos+=2;
    }
    else
    {
       Digit10(str[i]-'0',pos,y); 
       pos+=8;
    }
         i++;
    }
     if(ch=='h')
        {   
        Char('a',pos,y+2);
        pos+=6;
        }
     Char(ch,pos,y+2);
}
/*
void PrintValue2(unsigned char x,unsigned char y,unsigned short round,unsigned short frac,char ch)
{
unsigned char pos=x;
unsigned char i=0;
unsigned short tmp=0;
unsigned short val=round;

ClearBuf();
tmp= val/10;
if (tmp<10&&tmp>0)
{
Digit10(tmp,pos,y);
pos+=8;
}
else if(tmp>9)
{
 Digit10(tmp/10,pos,y);
pos+=8;
 Digit10(tmp%10,pos,y);
pos+=8;
}
Digit10(val%10,pos,y);
pos+=8;
lcdbuf[pos/8][8+y]|=192;
lcdbuf[pos/8][9+y]|=192;
pos+=2;
val=frac;
Digit10(val/100,pos,y);
pos+=8;
if (tmp<10)
{
Digit10(val/10%10,pos,y);
pos+=8;
}
if (!tmp)
{
Digit10(val%10,pos,y);
pos+=8;
}
if(ch=='h')
{

Char('a',pos,y+2);
pos+=6;
}
Char(ch,pos,y+2);
}
*/


void St7920Init()
{
delay_ms2(100);
    DDRD=0x08; 
    DDRC=1;
    PORTD=0x00;    
    DDRB=0xFF;
    delay_ms2(100);  
    SPSR|=0x00;
    SPCR=0x5D;
     //SPCR=0x54;
    delay_ms2(100);
    Send(0x20,0);
    Send(0x24,0);
	Send(0x26,0); 

	DisplayFullUpdate();
}