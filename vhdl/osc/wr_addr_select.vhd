library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;  

---- Uncomment the following library declaration if instantiating
---- any Xilinx primitives in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity wraddr is
	port(
		CLK : in std_logic;
		reset : in std_logic;
		dout : out std_logic_vector(9 downto 0)
	);
end wraddr;

architecture Behavioral of wraddr is

begin

clk_main: process(CLK,reset) 
variable var : integer range 0 to 799;
begin  
	 
	
	if reset='0' then
		dout<=(others=>'0');		
		var:=0;
	elsif  rising_edge(CLK) then
		if var=799 then
		var:=0;
		else
		var:=var+1;
		end if;
		dout<=std_logic_vector(to_unsigned(var,10));
	end if;
end process; 

end Behavioral;