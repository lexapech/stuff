#include "bike.h"

Bike::Bike()
{
	right = false;
	left = false;
	brake = false;
	mass = 10;
	b.pos = { 0,300 };
	b.momentum = { 0,100 };
	b.angle = 0 * M_PI / 180;
	b.L = 0;

	rear.mass = 1;
	front.mass = 1;
	rear.radius = 20;
	front.radius = 20;
	rear.b.pos = toGlobal({ -40,-10 });
	front.b.pos = toGlobal({ 40,-10 });
	rear.b.momentum = { 0,0 };
	front.b.momentum = { 0,0 };
	rear.b.L = 0;
	rear.b.angle = 0;
	front.b.L = 0;
	front.b.angle = 0;

	rear.s.p1 = { -30,0 };
	front.s.p1 = { 30,0 };
	rear.s.p2 = &rear;
	front.s.p2 = &front;

	rear.s.k = 10;
	front.s.k = 10;
	rear.s.l = 40;
	front.s.l = 40;
	rear.s.d = 1;
	front.s.d = 1;

	front.s.bike = this;
	rear.s.bike = this;
	rear.s.dir2 = toLocal(rear.b.pos) - rear.s.p1;
	front.s.dir2 = toLocal(front.b.pos) - front.s.p1;
}


void Bike::func(Bike* out, const Bike& in)
{
	//bike
	out->b.pos = in.b.momentum / in.mass;
	
	Vector2 rearLocal = in.rear.b.pos - in.b.pos;
	Vector2 frontLocal = in.front.b.pos - in.b.pos;

	//rear
	spring s = in.rear.s;
	Bike tb = in;
	s.bike = &tb;
	s.p2 = &tb.rear;
	out->rear. b.pos = in.rear.b.momentum / in.rear.mass;
	
	Vector2 rearF = s.force1() + (in.b.momentum / in.mass - Vector2{-rearLocal.y, rearLocal.x }*in.b.L - in.rear.b.momentum / in.rear.mass)* in.rear.s.d;;
 
	out->rear.b.momentum = rearF + s.force2() + Vector2(0, -9.81 * in.rear.mass);
	out->rear.b.L = 0;
	if (in.rear.b.L > -1 && in.left) out->rear.b.L = -100;
	if (in.rear.b.L < 5 && in.right) out->rear.b.L = 100;
	
	out->rear.b.angle = in.rear.b.L;
	//front
	spring s2 = in.front.s;
	s2.bike = &tb;
	s2.p2 = &tb.front;
	out->front.b.pos = in.front.b.momentum / in.front.mass;
	Vector2 frontF = s2.force1() + (in.b.momentum / in.mass - Vector2{-frontLocal.y, frontLocal.x}*in.b.L - in.front.b.momentum / in.front.mass) * in.front.s.d;
	out->front.b.momentum =  frontF + s2.force2() + Vector2(0, -9.81 * in.front.mass);
	out->front.b.L = 0;
	out->front.b.angle = in.front.b.L;

	if (in.brake)
	{
		out->rear.b.L = -in.rear.b.L*100;
		out->front.b.L = -in.front.b.L*100;
	}

	out->b.momentum = Vector2() - rearF - frontF  - s2.force2() - s.force2() +Vector2(0, -9.81 * in.mass);


	Vector2 rearjoint = in.rotate(s.p1,in.b.angle);
	
	Vector2 frontjoint = in.rotate(s2.p1, in.b.angle);

	double l = (Vector2::dot(s.force1(), { -rearjoint.y,rearjoint.x })
				+ Vector2::dot(s.force2(), { -rearLocal.y,rearLocal.x })
				 + Vector2::dot(s2.force1(), {-frontjoint.y,frontjoint.x})
				+ Vector2::dot(s2.force2(), { -frontLocal.y,frontLocal.x }))/in.mass*0.01;


	out->b.L = l;

	out->b.angle = in.b.L;
}


Bike Bike::rk( Bike* b)
{
	//Sleep(10);
	Bike res;
	double dt = 0.01;
	Bike t1;
	Bike t2;
	Bike t3;
	Bike t4;
	func(&t1, *b);
	func(&t2, *b + t1 * (dt * 0.5));
	func(&t3, *b + t2 * (dt * 0.5));
	func(&t4, *b + t3 * dt);
	return *b + (t1+t2*2+t3*2+t4) * (dt/6);
	
}

void Bike::update(int controls)
{
	switch (controls)
	{
	case -1: left = true; right = false; brake = false; break;
	case 0: left = false; right = false; brake = true; break;
	case 1: left = false; right = true; brake = false; break;
	case 2: left = false; right = false; brake = false; break;
	}

	double ground = -40;
	*this = rk(this);
	if (b.pos.y < ground) { b.pos.y = ground; b.momentum.y = 0; b.momentum.x *= 0.2;
	}

	if (rear.b.pos.y-rear.radius < ground) { 
		rear.b.pos.y = ground + rear.radius;
		rear.b.momentum.y = 0;
		double vel = (-rear.b.momentum.x / rear.mass + rear.b.L * rear.radius) * 0.05;
		rear.b.momentum.x += vel;
		rear.b.L -= vel/ rear.radius;
	}
	//
	if (abs(rear.b.pos.x)+rear.radius>500) {
		rear.b.pos.x = rear.b.pos.x / abs(rear.b.pos.x) * (500- rear.radius);
		rear.b.momentum.x = -rear.b.momentum.x;
		//std::cout << rear.b.momentum.x << "\n";
	}
	if (front.b.pos.y - front.radius < ground) {
		front.b.pos.y = ground + front.radius;
		front.b.momentum.y = 0;
		//front.b.momentum.x *= 0.8;
		double vel = (-front.b.momentum.x / front.mass + front.b.L * front.radius)*0.05;
		front.b.momentum.x += vel;
		front.b.L -= vel/ front.radius;
	}
	if (abs(front.b.pos.x) + front.radius > 500) {
		front.b.pos.x = front.b.pos.x / abs(front.b.pos.x) * (500 - front.radius);
		front.b.momentum.x = -front.b.momentum.x;

	}
	while (abs(b.angle) > 2 * M_PI) b.angle -= b.angle/abs(b.angle)*(2 * M_PI);
	while (abs(rear.b.angle) > 2 * M_PI) rear.b.angle -= rear.b.angle / abs(rear.b.angle) * (2 * M_PI);
	while (abs(front.b.angle) > 2 * M_PI) front.b.angle -= front.b.angle / abs(front.b.angle) * (2 * M_PI);
	//std::cout << b.angle << "\n";
	//std::cout << b.pos.y << "\n";
	
}


Vector2 spring::dir() const
{
	return (p2->b.pos - bike->toGlobal(p1)) / len();
}
Vector2 spring::mid() const
{
	return (p2->b.pos + bike->toGlobal(p1)) / 2;
}
double  spring::len() const
{
	return (p2->b.pos - bike->toGlobal(p1)).len();
}

double spring::angle() const
{
	return atan2(-dir().y,dir().x)/ M_PI *180;
}

Vector2 spring::force1() const
{

	return dir() * (len() - l) * -k;
}
Vector2 spring::force2() const
{
	return (p2->b.pos - (bike->toGlobal(p1) + bike->rotate(dir2, bike->b.angle) / dir2.len() * len())) * -k * 100;
}

Bike Bike::operator+(Bike r)
{
	Bike n = *this;
	n.b.pos = n.b.pos + r.b.pos;
	n.b.momentum = n.b.momentum + r.b.momentum;
	n.b.L = n.b.L + r.b.L;
	n.b.angle = n.b.angle + r.b.angle;

	n.rear.b.pos = n.rear.b.pos + r.rear.b.pos;
	n.rear.b.momentum = n.rear.b.momentum + r.rear.b.momentum;
	n.front.b.pos = n.front.b.pos + r.front.b.pos;
	n.front.b.momentum = n.front.b.momentum + r.front.b.momentum;

	n.rear.b.L = n.rear.b.L + r.rear.b.L;
	n.rear.b.angle = n.rear.b.angle + r.rear.b.angle;
	n.front.b.L = n.front.b.L + r.front.b.L;
	n.front.b.angle = n.front.b.angle + r.front.b.angle;

	return n;
}

Bike Bike::operator*(double k)
{
	Bike n = *this;
	n.b.pos = n.b.pos * k;
	n.b.momentum = n.b.momentum * k;
	n.b.L = n.b.L * k;
	n.b.angle = n.b.angle * k;



	n.rear.b.pos = n.rear.b.pos * k;
	n.rear.b.momentum = n.rear.b.momentum * k;
	n.front.b.pos = n.front.b.pos * k;
	n.front.b.momentum = n.front.b.momentum * k;

	n.rear.b.L = n.rear.b.L * k;
	n.rear.b.angle = n.rear.b.angle * k;
	n.front.b.L = n.front.b.L * k;
	n.front.b.angle = n.front.b.angle * k;


	return n;
}

Vector2 Bike::toGlobal(Vector2 v) const
{
	return b.pos + rotate(v, b.angle);
}
Vector2 Bike::rotate(Vector2 v, double angle) const
{
	return Vector2{ v.x * cos(angle) + v.y * sin(angle),
		   v.x * -sin(angle) + v.y * cos(angle) };
}
Vector2 Bike::toLocal(Vector2 v) const
{
	return rotate(v - b.pos, -b.angle);
}

