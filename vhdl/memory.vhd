library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;

entity memorypad is	  
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
end memorypad;	 

architecture memorypad of memorypad is	
type pad is array(0 to 31) of std_logic_vector(7 downto 0); 
signal memory : pad; 
	
begin  
	
clkout<=CLK;
	
reading: process(reset,memory,adr_a,adr_b) --datain

begin 
		
	a<=memory(to_integer(unsigned(adr_a)));
	b<=memory(to_integer(unsigned(adr_b)));
	
end process;


writing: process(reset,datain,adr_a,adr_wr) --datain

begin 
if reset='0' then 
		
		memory<=(others=>(others=>'0')); 
		--memory(1)<="00000001";
		--a<=(others=>'0');
		--b<=(others=>'0');	
	--elsif rising_edge(CLK) then	
	--clkout<='1'; 
	--elsif falling_edge(CLK) then
	--clkout<='0';
	
	else
		if (adr_wr=adr_a) and wr_en='1' then
			memory(to_integer(unsigned(adr_wr)))<=datain; 
		end if;	
	end if;
	
end process;
end memorypad;