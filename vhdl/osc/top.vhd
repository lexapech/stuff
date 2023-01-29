-------------------------------------------------------------------------------
--
-- Title       : No Title
-- Design      : processor
-- Author      : alex
-- Company     : ban
--
-------------------------------------------------------------------------------
--
-- File        : e:\My_Designs\workspace\processor\compile\top.vhd
-- Generated   : Mon Mar 25 18:17:25 2019
-- From        : e:\My_Designs\workspace\processor\src\top.bde
-- By          : Bde2Vhdl ver. 2.6
--
-------------------------------------------------------------------------------
--
-- Description : 
--
-------------------------------------------------------------------------------
-- Design unit header --
library IEEE;
use IEEE.std_logic_1164.all;


entity top is
  port(
       CLK : in STD_LOGIC;
       CLK2 : in STD_LOGIC;
       b : in STD_LOGIC_VECTOR(7 downto 0);
       reset : in STD_LOGIC;
		 hsync :out std_logic;
		 vsync : out std_logic;
		 color : out std_logic_vector(2 downto 0)
  );
end top;

architecture top of top is

---- Component declarations -----

component ramb4_s4_s4
  port (
			WEA : in STD_LOGIC;
			ENA : in STD_LOGIC;
			WEB : in STD_LOGIC;
			ENB : in STD_LOGIC;
			ADDRA : in STD_LOGIC_VECTOR(9 downto 0);
			ADDRB : in STD_LOGIC_VECTOR(9 downto 0);
			CLKA : in STD_LOGIC;
			CLKB : in STD_LOGIC;
			DIA : in STD_LOGIC_VECTOR(3 downto 0);
			DIB : in STD_LOGIC_VECTOR(3 downto 0);
			DOA : out STD_LOGIC_VECTOR(3 downto 0);
			DOB : out STD_LOGIC_VECTOR(3 downto 0)
  );
end component;
component test
  port (
       CLK : in STD_LOGIC;
		 din : in STD_LOGIC;
       reset : in STD_LOGIC;
       dout : out std_logic_vector(7 downto 0)
  );
end component;
component trigger
  port (
       CLK : in STD_LOGIC;
       a : in STD_LOGIC_VECTOR(7 downto 0);
       addrin : in STD_LOGIC_VECTOR(9 downto 0);
       b : in STD_LOGIC_VECTOR(7 downto 0);
       reset : in STD_LOGIC;
       addr : out STD_LOGIC_VECTOR(9 downto 0)
  );
end component;
component vga
  port (
       CLK : in STD_LOGIC;
       din1 : in STD_LOGIC_VECTOR(7 downto 0);
       din2 : in STD_LOGIC_VECTOR(7 downto 0);
       reset : in STD_LOGIC;
       addr : out STD_LOGIC_VECTOR(9 downto 0);
       color : out STD_LOGIC_VECTOR(2 downto 0);
       hsync : out STD_LOGIC;
       odd : out STD_LOGIC;
       vsync : out STD_LOGIC
  );
end component;
component wraddr
  port (
       CLK : in STD_LOGIC;
       reset : in STD_LOGIC;
       dout : out STD_LOGIC_VECTOR(9 downto 0)
  );
end component;

---- Signal declarations used on the diagram ----

--signal hsync : STD_LOGIC;
signal NET2165 : STD_LOGIC;
signal NET2344 : STD_LOGIC;
--signal vsync : STD_LOGIC;
signal addr : STD_LOGIC_VECTOR (9 downto 0);
signal BUS2070 : STD_LOGIC_VECTOR (7 downto 0);
signal BUS2079 : STD_LOGIC_VECTOR (9 downto 0);
signal BUS2106 : STD_LOGIC_VECTOR (7 downto 0);
signal BUS2115 : STD_LOGIC_VECTOR (7 downto 0);
signal BUS2155 : STD_LOGIC_VECTOR (9 downto 0);
--signal color : STD_LOGIC_VECTOR (2 downto 0);

begin

----  Component instantiations  ----

U1 : ramb4_s4_s4
  port map(
       ADDRB => BUS2155,
       ADDRA => BUS2079,
       CLKA => CLK,
		 CLKB => CLK,
       DIA => BUS2070(3 downto 0),
       DOB => BUS2115(3 downto 0),
       WEA => NET2344,
		 ENA =>'1',
		 ENB =>'1',
		 WEB=>'0',
		 DIB=>X"0"
  );

U11 : ramb4_s4_s4
  port map(
       ADDRB => BUS2155,
       ADDRA => BUS2079,
       CLKA => CLK,
		 CLKB => CLK,
       DIA => BUS2070(7 downto 4),
       DOB => BUS2115(7 downto 4),
       WEA => NET2344,
		 ENA =>'1',
		 ENB =>'1',
		 WEB=>'0',
		 DIB=>X"0"
  );

U2 : test
  port map(
		din => CLK2,
       CLK => CLK,
       dout => BUS2070,
       reset => reset
  );

U3 : wraddr
  port map(
       CLK => CLK,
       dout => BUS2079,
       reset => reset
  );

U4 : trigger
  port map(
       CLK => CLK,
       a => BUS2070,
       addr => addr,
       addrin => BUS2079,
       b => b,
       reset => reset
  );

U5 : ramb4_s4_s4
  port map(
       ADDRB => BUS2155,
       ADDRA => BUS2079,
       CLKA => CLK,
		 CLKB => CLK,
       DIA => BUS2070(3 downto 0),
       DOB => BUS2106(3 downto 0),
       WEA => NET2165,
		 ENA =>'1',
		 ENB =>'1',
		 WEB=>'0',
		 DIB=>X"0"
  );

U51 : ramb4_s4_s4
  port map(
       ADDRB => BUS2155,
       ADDRA => BUS2079,
       CLKA => CLK,
		 CLKB => CLK,
       DIA => BUS2070(7 downto 4),
       DOB => BUS2106(7 downto 4),
       WEA => NET2165,
		 ENA =>'1',
		 ENB =>'1',
		 WEB=>'0',
		 DIB=>X"0"
  );

U6 : vga
  port map(
       CLK => CLK,
       addr => BUS2155,
       color => color,
       din1 => BUS2106,
       din2 => BUS2115,
       hsync => hsync,
       odd => NET2344,
       reset => reset,
       vsync => vsync
  );

NET2165 <= not(NET2344);


end top;
