library IEEE;
use IEEE.STD_LOGIC_1164.all; 
use IEEE.numeric_std.all;  

entity vga is	  
	port(
	CLK : in std_logic;
	reset : in std_logic;
	din1 : in std_logic_vector(7 downto 0);
	din2 : in std_logic_vector(7 downto 0);
	color : out std_logic_vector(2 downto 0);
	hsync : out std_logic;
	vsync : out std_logic;
	odd : out std_logic;
	addr : out std_logic_vector(9 downto 0)
	);
end vga;

architecture vga of vga is	
type h is (hs,back,data,front);
type v is (vs,back,data,front);

signal line : h;
signal frame : v;
signal x : integer range 0 to 1023;	 
signal y : integer range 0 to 624;	
signal d2 : std_logic_vector(7 downto 0);
begin

clk_main: process(CLK,reset) 
variable col : std_logic_vector(2 downto 0);
variable od : std_logic; 
variable d : std_logic_vector(7 downto 0);

begin  
	 
	
	if reset='0' then
		line<=hs;
		frame<=vs; 
		hsync<='1';
		vsync<='1';	
		od:='0';
	elsif rising_edge(CLK) then	  
		case line is
			when hs => 	color<="000";
							hsync<='0';
							if x=71 then
								line<=back;									
							end if;
							x<=x+1;	
			when back => 	color<="000";
							hsync<='1';	
							if x=199 then
								line<=data;	
							end if;
							x<=x+1;	
			when data =>	if frame=data then		
							color<=col;
							else
							color<="000";
							end if;
							hsync<='1';	
							if x=999 then
								line<=front;	
							end if;
							x<=x+1;	
			when front => 	color<="000";
							hsync<='1';
							if x=1023 then
								line<=hs;
								x<=0; 
								if y/=624 then
									y<=y+1;
								else
									y<=0;
									vsync<='0';
								end if;
							else
								x<=x+1;	
							end if;
			when others =>				
		end case;
		case frame is 
			when vs => 			vsync<='0';									
								if y=1 and x=1023 then
														frame<=back;
								end if;
			when back => 		vsync<='1'; 
								if y=23 and x=1023 then
									frame<=data;
								end if;
			when data => 		vsync<='1';
								if y=623 and x=1023 then
									frame<=front;
								end if;
			when front => 		vsync<='1'; 
								if y=624 and x=1023 then
									frame<=vs;
									y<=0;
									od:=not(od);
								end if;
			when others =>
		end case; 
		if x>199 and x<1000 then			
		addr<=std_logic_vector(to_unsigned(x,10)-200); 
		end if;
		odd<=od; 
		if od='1' then
			d:=din1;
		else
			d:=din2;
		end if;
		--if d2<d then
			--if 623-to_integer(unsigned(d&"0"))<=y and 623-to_integer(unsigned(d2&"0"))>=y  then
			--col:="111";
			
			--else 
			--col:="000";
			--end if;
		--else
			--if 623-to_integer(unsigned(d&"0"))>=y and 623-to_integer(unsigned(d2&"0"))<=y  then
			--col:="111";
			
			--else 
			--col:="000";
			--end if;
		--end if;
		if 623-to_integer(unsigned(d&"0"))=y   then
			col:="111";
			
			else 
			col:="000";
		end if;
		d2<=d;
		
	end if;
	
end process; 


end vga;