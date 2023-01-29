----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date:    18:34:45 03/24/2019 
-- Design Name: 
-- Module Name:    test - Behavioral 
-- Project Name: 
-- Target Devices: 
-- Tool versions: 
-- Description: 
--
-- Dependencies: 
--
-- Revision: 
-- Revision 0.01 - File Created
-- Additional Comments: 
--
----------------------------------------------------------------------------------
library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;  

---- Uncomment the following library declaration if instantiating
---- any Xilinx primitives in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity test is
	port(
		CLK : in std_logic;
		din : in std_logic;
		reset : in std_logic;
		dout : out std_logic_vector( 7 downto 0)
	);
end test;

architecture Behavioral of test is

begin

clk_main: process(CLK,reset) 
variable var : unsigned(7 downto 0);
begin  
	 
	
	--if reset='0' then
		--dout<=(others=>'0');		
		--var:=(others=>'0');
	--elsif  rising_edge(CLK) then			
		--var:=var + unsigned'("00000001");
		--dout<=std_logic_vector(var);
	--end if;
	dout<=din&din&din&din&din&din&din&din;
end process; 

end Behavioral;

