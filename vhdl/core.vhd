library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;

entity core is	  
	port(
	CLK : in std_logic;
	reset : in std_logic;		 
	CMD : in std_logic_vector(15 downto 0);
	data :inout std_logic_vector(7 downto 0);
	adress: out std_logic_vector(6 downto 0)
	);
end core;	

architecture core of core is 

signal GPRa : std_logic_vector(4 downto 0);
signal GPRb : std_logic_vector(4 downto 0);	
signal GPRwr : std_logic_vector(4 downto 0);
signal a1 : std_logic_vector(7 downto 0);
signal a2 : std_logic_vector(7 downto 0);
signal latchin : std_logic_vector(7 downto 0);
signal latchout : std_logic_vector(7 downto 0);
signal b1 : std_logic_vector(7 downto 0);
signal b2 : std_logic_vector(7 downto 0); 
signal alucmd  : std_logic_vector(3 downto 0);
signal aluout : std_logic_vector(7 downto 0); 
signal memin : std_logic_vector(7 downto 0);
signal cmd2 : std_logic_vector(15 downto 0);
signal aluclk : std_logic;	
signal memclk : std_logic;	
signal wr_en : std_logic;	
component memorypad is	  
	port(
	CLK : in std_logic;
	reset : in std_logic;		
	wr_en : in std_logic; 
	adr_a : in std_logic_vector(4 downto 0);
	adr_b : in std_logic_vector(4 downto 0);
	adr_wr : in std_logic_vector(4 downto 0);
	a : out std_logic_vector(7 downto 0);
	b : out std_logic_vector(7 downto 0);
	datain : in std_logic_vector(7 downto 0);
	clkout : out std_logic
	);
end component;	 

component alu is	  
	port(
	CLK : in std_logic;
	reset : in std_logic;		
	cmd : in std_logic_vector(3 downto 0);
	a : in std_logic_vector(7 downto 0);
	b : in std_logic_vector(7 downto 0);
	dataout : out std_logic_vector(7 downto 0)
	);
end component;

begin  
	
mem : memorypad port map(a=>a1, b=>b1, datain=>memin, CLK=>CLK, clkout=>aluclk, reset=>reset, wr_en=>wr_en, adr_a=>GPRa, adr_b=>GPRb,adr_wr=>GPRwr);
alu0 : alu port map(a=>a2,b=>b2, dataout=>aluout, CLK=>CLK, cmd=>alucmd, reset=>reset);
	
	
clk_main: process(CLK,reset,a1,a2,aluout,CMD)	
variable cm1: std_logic_vector(5 downto 0);
variable cm2: std_logic_vector(7 downto 0);
variable cm3: std_logic_vector(3 downto 0);
variable cm4: std_logic_vector(10 downto 0);
variable cm5: std_logic_vector(12 downto 0);
variable cm6: std_logic_vector(7 downto 0);
variable cm7: std_logic_vector(5 downto 0);
variable cm8: std_logic_vector(15 downto 0);
variable cm9: std_logic_vector(4 downto 0);
variable cm10: std_logic_vector(9 downto 0);
variable cm11: std_logic_vector(7 downto 0);
variable cm12: std_logic_vector(3 downto 0);
variable cm13: std_logic_vector(7 downto 0);
variable cm14: std_logic_vector(4 downto 0);
variable wr_en1: std_logic;
begin  
	cm1:=cmd(15 downto 10);
	cm2:=cmd(15 downto 8); 
	cm3:=cmd(15 downto 12);
	cm4:=cmd(15 downto 9)&cmd(3 downto 0);
	cm5:=cmd(15 downto 7)&cmd(3 downto 0);
	cm6:=cmd(15 downto 9)&cmd(3);
	cm7:=cmd(15 downto 10);	
	cm8:=cmd; 
	cm9:=cmd(15 downto 11);
	cm10:=cmd(15 downto 7)&cmd(3);
	cm11:=cmd(15 downto 8);
	cm12:=cmd(15 downto 12);
	cm13:=cmd(15 downto 8);
	cm14:=cmd(15 downto 14)&cmd(12)&cmd(9)&cmd(3);
	data<=memin;
	
	--cmd2<=cmd;
	if reset='0' then
		--a1 <=(others=>'0');
		a2 <=(others=>'0');
		--b1 <=(others=>'0');
		b2 <=(others=>'0');
		alucmd <=(others=>'0');
		--aluout <=(others=>'0');
		memin <=(others=>'0');
	end if;	
		
		case cm1 is
			when "000111" => 	alucmd<="0010";	--ADC	
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								memin<=aluout; 
			when "000011" =>  	alucmd<="0000";	 --ADD 
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;	
								memin<=aluout;
			when "001000" =>   	alucmd<="0100";	 --AND	
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								memin<=aluout;
			when "000101" =>	alucmd<="0001";	 --CP	
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								wr_en1:='0';
			when "000001" =>   	alucmd<="0011";	 --CPC	
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								wr_en1:='0';		
			when "000100" =>					--CPSE
			when "001011" =>	alucmd<="1111";
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								b2<=b1;
								memin<=aluout;												--MOV
			when "100111" =>					--MUL
			when "001010" =>   	alucmd<="0101";	 --OR 
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								memin<=aluout;
			when "000010" =>   	alucmd<="0011";	 --SBC	
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;	
								memin<=aluout;
			when "000110" =>   	alucmd<="0001";	 --SUB
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								memin<=aluout;
			when "001001" => 	alucmd<="0110";	 --EOR 
								GPRa<=cmd(8 downto 4);
								GPRb<=cmd(9)&cmd(3 downto 0);
								a2<=a1;
							 	b2<=b1;
								memin<=aluout;
			when others   =>
		end case; 
		
		case cm2 is	  
			when "10010110"	=>	  --ADIW
			when "10010111"	=>	  --SBIW
			when others =>
		end case;  
		case cm3 is	  
			when "1100"	=>	  --RJMP
			when "1101"	=>	  --RCALL
			when others =>
		end case;
		case cm4 is	  
			when "10010100101"	=>	alucmd<="1000";	
									GPRa<=cmd(8 downto 4);
									GPRb<=cmd(8 downto 4);
							 		b2<=b1;
									memin<=aluout; 	  --ASR
			when "10010100000"	=>	alucmd<="1001";	
									GPRa<=cmd(8 downto 4);
									GPRb<=cmd(8 downto 4); 
									a2<="11111111";
							 		b2<=b1;
									memin<=aluout;   --COM
			when "10010101010"	=>	alucmd<="0001";	
									GPRa<=cmd(8 downto 4);
									--GPRb<=cmd(8 downto 4); 
									a2<=a1;
							 		b2<="00000001";
									memin<=aluout;  --DEC
			when "10010100011"	=>	alucmd<="0000";		
									GPRa<=cmd(8 downto 4);
									GPRb<=cmd(8 downto 4);
									b2<=b1;
							 		a2<="00000001";
									memin<=aluout;  --INC
			when "10010100110"	=>	alucmd<="1010";		
									GPRb<=cmd(8 downto 4);
									GPRa<=cmd(8 downto 4);
							 		b2<=b1;
									memin<=aluout;  --LSR
			when "10010100001"	=>	alucmd<="0001";	
									GPRa<=cmd(8 downto 4);
									GPRb<=cmd(8 downto 4); 
									a2<="00000000";
							 		b2<=b1;
									memin<=aluout; 	  --NEG
			when "10010001111"	=>	  --POP
			when "10010011111"	=>	  --PUSH
			when "10010100111"	=>	alucmd<="1011";	
									GPRa<=cmd(8 downto 4);
									GPRb<=cmd(8 downto 4);
							 		b2<=b1;
									memin<=aluout;  --ROR
			when "10010100010"	=>	alucmd<="1100";	
									GPRa<=cmd(8 downto 4);
									GPRb<=cmd(8 downto 4);
							 		b2<=b1;
									memin<=aluout;  --SWAP 
			when "10000000000"	=>	  --LD
			when "10000010000"	=>	  --ST
			when "10010000100"	=>	  --LPM
			when "10010000101"	=>
			when "10010000110"	=>	  --ELPM
			when "10010000111"	=>
			when others =>
		end case;
		case cm5 is	  
			when "1001010011000"	=>	  --BCLR
			when "1001010001000"	=>	  --BSET
			when others =>
		end case;
		case cm6 is	  
			when "11111000"	=>	  --BLD
			when "11111010"	=>		--BST
			when "11111100"	=>	  --SBRC
			when "11111110"	=>		--SBRS
			when others =>
		end case;
		case cm7 is	  
			when "111100"	=>	  --BRBS
			when "111101"	=>	  --BRBC
			when others =>
		end case;
		case cm8 is	  
			when X"0000"	=>	  --NOP
			when X"9409"	=>	  --IJMP  
			when X"9508"	=>	  --RET
			when X"9518"	=>	  --RETI
			when X"9588"	=>	  --SLEEP
			when X"95A8"	=>	  --WDR	
			when X"95C8"	=>	  --LPM
			when X"95E8"	=>	  --SPM
			when X"9509"	=>	  --ICALL
			when X"95D8"	=>	  --ELPM
			when others =>
		end case;
		case cm9 is	  
			when "10110"	=>	data<=(others=>'Z');
								GPRa<=cmd(8 downto 4);
								if cmd(10 downto 9)&cmd(3 downto 0)="111111" then
								
								alucmd<="1101";
								else
								alucmd<="1111";
								b2<=data;								
								adress<=cmd(10 downto 9)&cmd(3 downto 0)&"0";--IN 
								end if;
								memin<=aluout;
			when "10111"	=>	GPRb<=cmd(8 downto 4);
								if cmd(10 downto 9)&cmd(3 downto 0)="111111" then
								b2<=b1;
								alucmd<="1110";
								else
								--data<=b1;								
								adress<=cmd(10 downto 9)&cmd(3 downto 0)&"1"; --OUT	 
								end if;
			when others =>
		end case;  
		case cm10 is	  
			when "0000001100"	=>	  --MUL
			when "0000001101"	=>	  --FMUL
			when others =>
		end case; 
		case cm11 is	  
			when "00010000"	=>	  --MUL
			when others =>
		end case;
		case cm12 is	  
			when "0011"		=>	alucmd<="0001";		
								GPRa<="1"&cmd(7 downto 4);
								a2<=a1;
								b2<=cmd(11 downto 8)&cmd(3 downto 0); --CPI
								wr_en1:='0';
			when "0100"		=>	alucmd<="0011";		
								GPRa<="1"&cmd(7 downto 4);
								a2<=a1;
								b2<=cmd(11 downto 8)&cmd(3 downto 0);
								memin<=aluout;  --SBCI
			when "0101"		=>	alucmd<="0001";		
							GPRa<="1"&cmd(7 downto 4);
							a2<=a1;
							b2<=cmd(11 downto 8)&cmd(3 downto 0);
							memin<=aluout;  --SUBI
			when "0110"		=>	alucmd<="0101";		
							GPRa<="1"&cmd(7 downto 4);
							a2<=a1;
							b2<=cmd(11 downto 8)&cmd(3 downto 0);
							memin<=aluout;  --ORI
			when "0111"		=>	alucmd<="0100";		
							GPRa<="1"&cmd(7 downto 4);
							a2<=a1;
							b2<=cmd(11 downto 8)&cmd(3 downto 0);
							memin<=aluout;  --ANDI
			when "1110"		=>	GPRa<="1"&cmd(7 downto 4);
									GPRb<="1"&cmd(7 downto 4);
							alucmd<="1111";
							b2<=cmd(11 downto 8)&cmd(3 downto 0);  --LDI
							memin<=aluout;
			when others =>
		end case;
		if rising_edge(CLK) then
			GPRwr<=GPRa; 
			if cm1="000101" or cm1="000001" or cm12="0011" then
				wr_en<='0';
			else
				wr_en<='1';
			end if;
			
			--wr_en<=wr_en1;			
		end if;
end process;
end core;