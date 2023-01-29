library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;

entity alu is	  
	port(
	CLK : in std_logic;
	reset : in std_logic;		
	cmd : in std_logic_vector(3 downto 0);
	a : in std_logic_vector(7 downto 0);
	b : in std_logic_vector(7 downto 0);
	dataout : out std_logic_vector(7 downto 0)
	);
end alu;

architecture alu of alu is 
signal sreg : std_logic_vector(7 downto 0);
signal cm : std_logic_vector(3 downto 0);
begin
 
cm<=cmd;

clk_main: process(CLK,reset)	
	
variable most : unsigned(4 downto 0);
variable least : unsigned(4 downto 0);
variable Rb : unsigned(7 downto 0);	 	
variable status : unsigned(7 downto 0);
variable c : std_logic;	 
variable temp : std_logic;	


begin 
	
	if reset='0' then
	dataout<=(others=>'0');
	sreg<=(others=>'0');
	
	elsif rising_edge(CLK) then  
		status:=unsigned(sreg);	  
		if cm="1001" or cm(3)='0' then
			if cm(2) = '0' then
				if cm(1) = '0' then
					c := '0';
				elsif cm(1) = '1' then
					c := sreg(0);
				end if;
				if cm(0) = '0' then
					Rb := unsigned(b); 
				elsif cm(0) = '1' then
					Rb := unsigned(not(b));
					c := not(c);
				end if;	
			
				least := "0" & unsigned(a(3 downto 0)) +  Rb(3 downto 0) + unsigned'("0000" & c);
				most  := "0" & unsigned(a(7 downto 4)) +  Rb(7 downto 4) + unsigned'("0000" & least(4));
				--dataout <= std_logic_vector(most(3 downto 0) & least(3 downto 0));
				
				if cm(0) = '1' then	 --00x1
					status(0):=not(most(4));
					status(5):=not(least(4)); 
					status(3):=(a(7) xor b(7)) and not(most(3));
				else				 --00x0
					status(0):=most(4);
					status(5):=least(4);
					status(3):=(a(7) nor Rb(7)) and most(3);
				end if;
				if cm = "1001" then
					status(0) := '1';
				end if;
					
				
			elsif cm(2) = '1' then 	  --01xx
				case cm(1 downto 0) is
					when "00" => 	least:="0"&unsigned(a(3 downto 0) and b(3 downto 0));
									most:="0"&unsigned(a(7 downto 4) and b(7 downto 4));
									
					when "01" => 	least:="0"&unsigned(a(3 downto 0) or b(3 downto 0));
									most:="0"&unsigned(a(7 downto 4) or b(7 downto 4));
									
					when "10" => 	least:="0"&unsigned(a(3 downto 0) xor b(3 downto 0));
									most:="0"&unsigned(a(7 downto 4) xor b(7 downto 4));
														
				 	when others =>
				end case; 
				--dataout <=std_logic_vector( most(3 downto 0) & least(3 downto 0));
				status(3):='0';
			end if;	
			
			
		else  
			if cm(2)='0' then
			case cm(1 downto 0) is
				when "00" => temp:=b(7);
				when "10" => temp:='0';
				when "11" => temp:=sreg(0);																							
				when others =>				
			end case;
			least:="0" & unsigned(b(4 downto 1)); 
			most:="0" &temp& unsigned(b(7 downto 5));	
			
			status(0):=b(0);  
			status(4):=	most(3) xor status(0);
			else 
				case cm(1 downto 0) is
					when "00" => most:= "0"&unsigned(b(3 downto 0));
								least:= "0"&unsigned(b(7 downto 4));
					when "01" => most:="0"&unsigned(sreg(7 downto 4));
								least:= "0"&unsigned(sreg(3 downto 0));
					when "10"=> status:=unsigned(b);
								sreg<=std_logic_vector(status);
					when "11"=> most:="0"&unsigned(b(7 downto 4));
								least:= "0"&unsigned(b(3 downto 0));
								sreg<=std_logic_vector(status);								
					when others =>
				end case;
			end if;	
		end if;	
		if (cm/="1100") and (cm/="1110") and (cm/="1101") and (cm/="1111") then
		status(2):=most(3);
		status(4):=	status(2) xor status(3);
		status(1):=	not(most(3)or most(2)or most(1)or most(0)or least(3)or least(2)or least(1)or least(0));	
		sreg<=std_logic_vector(status);	
		end if;
		dataout <=std_logic_vector( most(3 downto 0) & least(3 downto 0));	
		
		 
		
	end if;	
  
	
	--dataout <=std_logic_vector( most(3 downto 0) & least(3 downto 0));
	--dataout <=std_logic_vector( most(3 downto 0) & least(3 downto 0));
end process;
end alu;