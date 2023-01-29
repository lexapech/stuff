library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;  

---- Uncomment the following library declaration if instantiating
---- any Xilinx primitives in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity trigger is
	port(
		CLK : in std_logic;
		reset : in std_logic; 
		a : in std_logic_vector(7 downto 0);
		b : in std_logic_vector(7 downto 0);  
		addrin : in std_logic_vector(9 downto 0);
		addr : out std_logic_vector(9 downto 0)
	);
end trigger;

architecture Behavioral of trigger is
signal comp : std_logic;
begin

clk_main: process(CLK,reset) 
begin  
	 
	
	if reset='0' then
		addr<=(others=>'0');		

	elsif  rising_edge(CLK) then
		if a>b then
			comp<='1';
		else
			comp<='0';
		end if;
	end if;
	addr<=(others=>'0');
end process; 

end Behavioral;