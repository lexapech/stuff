#include "func.h"
	Vector3d Vector3d::normalize()
	{
		Vector3d res;
		double len = sqrt(x * x + y * y + z * z);
		res = *this / len;
		return res;
	}
	double Vector3d::magnitude()
	{
		return sqrt(x * x + y * y + z * z);
	}


	Vector3d Vector3d::operator+(const Vector3d rv) const
	{
		return Vector3d(x + rv.x, y + rv.y, z + rv.z);
	}
	Vector3d Vector3d::operator-(const Vector3d rv) const
	{
		return Vector3d(x - rv.x, y - rv.y, z - rv.z);
	}
	Vector3d Vector3d::operator/(const double rv) const
	{
		return Vector3d(x / rv, y / rv, z / rv);
	}
	Vector3d Vector3d::operator*(const double rv) const
	{
		return Vector3d(x * rv, y * rv, z * rv);
	}
	Vector3d Vector3d::cross(const Vector3d l, const Vector3d r)
	{
		Vector3d res;
		res.x = l.y * r.z - l.z * r.y;
		res.y = l.z * r.x - l.x * r.z;
		res.z = l.x * r.y - l.y * r.x;
		return res;
	}
	double& Vector3d::operator[](int index)
	{
		switch (index)
		{
		case 0:
			return x;
			break;
		case 1:
			return y;
			break;
		case 2: 
			return z;
			break;
		}
	}
	double Vector3d::dot(const Vector3d l, const Vector3d r)
	{
		return l.x * r.x + l.y * r.y + l.z * r.z;
	}

	Quaternion::Quaternion(double real, Vector3d img) :re(real), im(img) {};
	Quaternion Quaternion::operator*(const Quaternion r) const
	{
		Quaternion q;
		q.re = re * r.re - Vector3d::dot(im, r.im);
		q.im = r.im * re + im * r.re + Vector3d::cross(im, r.im);
		
		/*
		q.re = re * r.re - im.x * r.im.x - im.y * r.im.y - im.z * r.im.z;
		q.im.x = re * r.im.x + im.x * r.re + im.y * r.im.z - im.z * r.im.y;
		q.im.y = re * r.im.y - im.x * r.im.z + im.y * r.re + im.z * r.im.x;
		q.im.z = re * r.im.z + im.x * r.im.y - im.y * r.im.x + im.z * r.re;
		*/
		return q;
	}
	Quaternion Quaternion::operator*(const double rv) const
	{
		return Quaternion(re * rv, im * rv);
	}
	Quaternion Quaternion::normalize()
	{
		Quaternion res;
		double len = sqrt(re * re + im.x * im.x + im.y * im.y + im.z * im.z);
		res.re = re / len;
		res.im = im / len;
		return res;
	}


	Matrix3x3 Matrix3x3::T() const
	{
		Matrix3x3 c;
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j)
			{
				c.set(get(i, j), j, i);
			}
		}
		return c;
	}
	Matrix3x3::Matrix3x3(double t)
	{
		for (int i = 0; i < 9; i++) arr[i] = 0;
		arr[0] = t;
		arr[4] = t;
		arr[8] = t;
	}
	Matrix3x3::Matrix3x3(double Arr[])
	{
		std::memcpy(arr, Arr, 9 * sizeof(double));
	}
	void Matrix3x3::set(double e, int x, int y) {
		arr[x + 3 * y] = e;
	}
	double Matrix3x3::get(int x, int y) const {
		return arr[x + 3 * y];
	}
	Matrix3x3 Matrix3x3::operator=(Matrix3x3 m)
	{
		std::memcpy(arr, m.arr, 9 * sizeof(double));
		return *this;
	}
	Vector3d Matrix3x3::operator*(Vector3d v) const {
		Vector3d c;//i -строка j-столбец
		for (int i = 0; i < 3; ++i)
		{		
			double sum = 0;
			for (int k = 0; k < 3; ++k)
				sum += this->get(i, k) * v[k];
			c[i] = sum;
		}
		return c;
	}
	Matrix3x3 Matrix3x3::operator*(Matrix3x3 m) const {
		Matrix3x3 c;
		for (int i = 0; i < 3; ++i)
		{
			for (int j = 0; j < 3; ++j)
			{
				double sum = 0;
				for (int k = 0; k < 3; ++k)
					sum += this->get(i, k) * m.get(k, j);
				c.set(sum, i, j);
			}
		}
		return c;
	}
	Matrix3x3 Matrix3x3::operator+(Matrix3x3 m) const {
		double t[9];
		for (int i = 0; i < 9; i++)
		{
			t[i] = arr[i] + m.arr[i];
		}
		return Matrix3x3(t);
	}
	Matrix3x3 Matrix3x3::operator*(double k) const {
		double t[9];
		for (int i = 0; i < 9; i++)
		{
			t[i] = arr[i] * k;
		}
		return Matrix3x3(t);
	}
	Matrix3x3 Matrix3x3::crossPMatrix(Vector3d v)
	{
		double t[9];
		t[0] = 0;
		t[1] = -v.z;
		t[2] = v.y;
		t[3] = v.z;
		t[4] = 0;
		t[5] = -v.x;
		t[6] = -v.y;
		t[7] = v.x;
		t[8] = 0;
		return Matrix3x3(t);
	}
	double Matrix3x3::adj(int x, int y) const
	{
		double t[4] = {0,0,0,0};
		int index = 0;
		for (int j = 0; j < 3; j++)
		{
			for (int i = 0; i < 3; i++)
			{
				if (j != y && i != x)
				{
					t[index] = get(i, j);
					index++;
				}
			}
		}
		double det = t[0] * t[3] - t[1] * t[2];
		return (1-((x+y)%2)*2) * det;
	}
	double Matrix3x3::det() const
	{
		return get(0, 0)* adj(0, 0) + get(1, 0) * adj(1, 0) + get(2, 0) * adj(2, 0);
	}
	Matrix3x3::Matrix3x3(Quaternion q)
	{		
		arr[0] = 1 -	2 * q.im.y * q.im.y - 2 * q.im.z * q.im.z;
		arr[1] =		2 * q.im.x * q.im.y - 2 * q.im.z * q.re;
		arr[2] =		2 * q.im.x * q.im.z + 2 * q.im.y * q.re;

		arr[3] =		2 * q.im.x * q.im.y + 2 * q.im.z * q.re;
		arr[4] = 1 -	2 * q.im.x * q.im.x - 2 * q.im.z * q.im.z;
		arr[5] =		2 * q.im.y * q.im.z - 2 * q.im.x * q.re;

		arr[6] =		2 * q.im.x * q.im.z - 2 * q.im.y * q.re;
		arr[7] =		2 * q.im.y * q.im.z + 2 * q.im.x * q.re;
		arr[8] = 1 -	2 * q.im.x * q.im.x - 2 * q.im.y * q.im.y;
	}



	Matrix3x3 Matrix3x3::Inv(Matrix3x3 m)
	{
		double d = m.det();
		double t[9];
		for (int j = 0; j < 9; j++)
		{
			t[j] = m.adj(j % 3, j / 3);
		}
		Matrix3x3 rt = Matrix3x3(t).T()*(1/d);
		return rt;
	}
	Matrix3x3 Matrix3x3::InvDiag(Matrix3x3 m)
	{
		double t[9] = { 1/m.get(0,0),0,0,0,1 / m.get(1,1),0,0,0,1 / m.get(2,2) };
		return Matrix3x3(t);
	}

	RigidBody::RigidBody(Vector3d size) : size(size)
	{
		double density = 10;
		points.push_back(Vector3d{-0.5*size.x,-0.5*size.y,-0.5*size.z });
		points.push_back(Vector3d{ 0.5*size.x,-0.5*size.y,-0.5*size.z });
		points.push_back(Vector3d{ 0.5*size.x,-0.5*size.y, 0.5*size.z });
		points.push_back(Vector3d{-0.5*size.x,-0.5*size.y, 0.5*size.z });
		points.push_back(Vector3d{-0.5*size.x,0.5 *size.y,-0.5*size.z });
		points.push_back(Vector3d{ 0.5*size.x,0.5 *size.y,-0.5*size.z });
		points.push_back(Vector3d{ 0.5*size.x,0.5 *size.y,0.5 *size.z});
		points.push_back(Vector3d{-0.5*size.x,0.5 *size.y,0.5 *size.z});
		double total = 0;
		centerofmass = Vector3d();
		for (int i = 0; i < points.size(); i++)
		{
			centerofmass = centerofmass + points[i];
		}
		centerofmass = centerofmass / points.size();
		prop.mass = size.x*size.y*size.y*density;
	}

	Vector3d RigidBody::getPointGlobalPos(Vector3d point)
	{
		return state.pos + state.R * (point-centerofmass);
	}
	Matrix3x3 RigidBody::CalcInertia()
	{
		/*
		double t[9] = {0,0,0,0,0,0,0,0,0};
		for (int i=0; i < points.size(); i++)
		{
			double x = points[i].x-centerofmass.x;
			double y= points[i].y - centerofmass.y;
			double z = points[i].z - centerofmass.z;
			double pmass = pointmasses[i];
			t[0] += (y * y + z * z)*pmass;
			t[1] -= (x * y)*pmass;
			t[2] -= (x * z) * pmass;
					
			t[3] -= (y * x) * pmass;
			t[4] += (x * x + z * z) * pmass;
			t[5] -= (y * z) * pmass;
					
			t[6] -= (z * x) * pmass;
			t[7] -= (z * y) * pmass;
			t[8] += (x * x + y * y) * pmass;

		}
		return Matrix3x3(t);
		*/
		double x = size.x;
		double y = size.y;
		double z = size.z;
		double t[9] = { y*y+z*z,0,0,0,x*x+z*z,0,0,0,x*x+y*y };
		return Matrix3x3(t)*(prop.mass/12);

	}

	Vector3d RigidBody::PointVel(Vector3d p)
	{
		Vector3d r = getPointGlobalPos(p) - getPointGlobalPos(centerofmass);
		//Vector3d r = p - centerofmass;
		Vector3d omega = state.R * prop.Inertia * state.R.T() * state.AngularM;

		return state.impulse / prop.mass + Vector3d::cross(omega, r);
		//return state.impulse / prop.mass + Vector3d::cross(r, (getPointGlobalPos(prop.Inertia  * state.AngularM)- getPointGlobalPos(centerofmass)));
		//return state.impulse / prop.mass + state.R * Vector3d::cross(r, prop.Inertia *  state.AngularM );
	}
	Vector3d RigidBody::PointVel2(Vector3d p)
	{
		Vector3d r = p - centerofmass;
		Vector3d IL = prop.Inertia * state.AngularM;
		Vector3d rot = Vector3d::cross(r, IL);
		return state.impulse / prop.mass + state.R * rot;
	}

	std::vector<Vector3d> RigidBody::touchPoint(double y)
	{
		std::vector<Vector3d> point;
		for (auto i = points.begin(); i != points.end(); ++i)
		{

			double pos = getPointGlobalPos(*i).y;
			double vrel = Vector3d::dot(Vector3d(0, 1, 0), PointVel2(*i));
			//if (std::abs(pos - y)<0.01 || pos<0)
			if (std::abs(pos - y) < 0.01  && vrel < 0)
			{
				point.push_back(*i);
				//return true;
			}
		}
		return point;
	}




	State State::operator*(double r)
	{
		Quaternion n = Quaternion(1, q.im * r);
		return { pos * r,impulse * r,n   /* n*/, AngularM * r};
	}
	State State::operator+(State r)
	{
		Quaternion n = (r.q * q).normalize();
		//Quaternion n = q.normalize();
		return { pos +r.pos,impulse + r.impulse,n, AngularM + r.AngularM,Matrix3x3(n) };
	}


	void cl::grav(State& res,const State& inp,const properties& cont)
	{
		res.pos = inp.impulse / cont.mass;
		res.impulse = cont.forceDir;

		//res.q = Quaternion(1, inp.R * cont.Inertia * inp.R.T() * inp.AngularM) * 0.5;
		//Vector3d omega = inp.R * cont.Inertia * inp.R.T() * inp.AngularM;
		Vector3d omega = cont.Inertia * inp.AngularM;
		//Vector3d omega = inp.R * cont.Inertia * inp.AngularM;
		res.q = Quaternion(0, omega) * 0.5;
		//res.AngularM = Vector3d::cross(cont.forcePoint,cont.forceDir);
	}

	RigidBody cl::Euler(RigidBody initial, double dt)
	{
		double time = 0;
		State t;
		grav(t, initial.state,initial.prop);
		time += dt;
		initial.state = initial.state + t * dt;
		return initial;
	}
	RigidBody cl::Midp(RigidBody initial, double dt)
	{
		double time = 0;
		State t;
		grav(t, initial.state, initial.prop);
		time += dt;
		State k1;
		grav(k1, initial.state, initial.prop);
		grav(t, initial.state + k1*(dt/2), initial.prop);
		initial.state = initial.state + t * dt;
		return initial;
	}
	RigidBody cl::RK4(RigidBody body, double dt)
	{
		//double time = 0;
		State k1;
		State k2;
		State k3;
		State k4;

		//state ->dt/2
		grav(k1, body.state , body.prop);		
		grav(k2, body.state + k1 * (dt / 2), body.prop);
		grav(k3, body.state + k2 * (dt / 2), body.prop);
		grav(k4, body.state + k3 * dt, body.prop);
		//body.state = body.state + (k1 + k2 * 2 + k3 * 2 + k4) * (dt / 6);
		//body.state = body.state + (k1 + k2 + k2 + k3 + k3 + k4) * (dt / 6);
		body.state = body.state + k1 * (dt / 6);
		body.state = body.state + k2 * (dt / 3);
		body.state = body.state + k3 * (dt / 3);
		body.state = body.state + k4 * (dt / 6);


		//time += dt;
		return body;
	}

	void cl::Plane()
	{
		glColor3f(0.1f, 1, 0.05f);

		glBegin(GL_QUADS);
		glVertex3f(-10, 0, -10);
		glVertex3f(10, 0, -10);
		glVertex3f(10, 0, 10);
		glVertex3f(-10, 0, 10);
		glEnd();
	}
	void  RigidBody::draw()
	{
		Vector3d light = Vector3d(1, -1, 0).normalize();
		GLUquadric* gluq = gluNewQuadric();
		glColor3f(1, 1, 1);
		int quads[] = { 0,1,2,3,
						0,3,7,4,//0347
						1,2,6,5,//1256
						0,1,5,4,//0145
						2,3,7,6,//2367
						4,5,6,7 };
		
		glBegin(GL_QUADS);
		glPushMatrix();
		for (int i = 0; i < 24; i++)
		{
			int side = i / 4;
			
			Vector3d p1 = points[quads[side * 4]];
			Vector3d p2 = points[quads[side * 4 + 1]];
			Vector3d p3 = points[quads[side * 4 + 2]];
			Vector3d p4 = points[quads[side * 4 + 3]];
			Vector3d norm = (getPointGlobalPos((p1 + p2 + p3 + p4)) -getPointGlobalPos((Vector3d()))).normalize();
			float c = 0.5 + 0.5 * std::max(-0.2,(-Vector3d::dot(light, norm)));
			glColor3f(c, c, c);
			Vector3d temp = getPointGlobalPos(points[quads[i]]);
			glVertex3f(temp.x, temp.y, temp.z);
		}
		glPopMatrix();
		glEnd();		
	}


void RigidBody::step(double dt)
{
	double K = std::pow(state.impulse.magnitude(), 2) / (2 * prop.mass);
	Vector3d omega = prop.Inertia * state.AngularM;
	double T = 0.5 * Vector3d::dot(omega, CalcInertia() * omega);
	double P = prop.mass * 9.81 * getPointGlobalPos(centerofmass).y;
	//std::cout << "total energy "<< K+T+P <<"\n";
	//std::cout << "dt " << dt << "\n";
	printf("total energy %.16le\n", K + T + P);
	double epsilon = 0.01;
	double planeh = 0;
	double newdt = dt;
	RigidBody newb = cl::RK4(*this, dt);
	
	double currlowest = getPointGlobalPos(lowestPoint()).y;
	double newlowest = newb.getPointGlobalPos(newb.lowestPoint()).y;
	double newvel = newb.PointVel2(newb.lowestPoint()).y;
	double vel = newlowest - currlowest;	
	if (newlowest> planeh + epsilon) //step if higher or moving away
	{
		*this = newb;
	}
	else if (newlowest < planeh + epsilon)  //if under try to decrease step
	{
		std::cout << "overshoot: " << newlowest << "\n";
		Vector3d lpp = newb.lowestPoint();
		std::cout << lpp.x << " " << lpp.y << " " << lpp.z << "\n";
		//std::cout << "newl " << getPointGlobalPos(Vector3d(-1, -0.5, -0.5)).y << " vel " << newvel << "\n";

		while (std::abs(newlowest - planeh) > epsilon)
		{
			newdt = newdt / 2;
			newb = cl::RK4(*this, newdt);
			Vector3d lp = newb.lowestPoint();
			newlowest = newb.getPointGlobalPos(lp).y;
			std::cout << "lowest iter: " << lp.x << " " << lp.y << " " << lp.z << " " << newb.PointVel2(lp).y << " " << newlowest << "\n";
			if (newlowest > 0) *this = newb;
			//if (newb.PointVel2(lp).y > 0) break;

		}
		std::cout << "after correction: " << newlowest << "\n";
		newb.contact(0);
		*this = newb;
	}

	
}


Vector3d RigidBody::lowestPoint()
{
	Vector3d lowest = centerofmass;
	for (auto it = points.begin(); it != points.end(); it++)
	{
		if (getPointGlobalPos(*it).y < getPointGlobalPos(lowest).y)
		{
			lowest = *it;
		}
	}
	return lowest;
}

void RigidBody::contact(double h)
{
	std::vector<Vector3d> p = touchPoint(h);
	if (p.size() > 0)
	{
		int cnt = 0;
		std::cout << "collisions " << p.size() << "\n";
		Vector3d point;
		for (auto it = p.begin(); it != p.end(); it++)
		{
			point = point + *it;
		}
		point = point / p.size();
		std::cout << point.x << " " << point.y << " " << point.z << "\n";
		//std::cout <<"touch point " << getPointGlobalPos(*it).y << "\n";
		Vector3d n = Vector3d(0, 1, 0);
		Vector3d r = getPointGlobalPos(point) - getPointGlobalPos(centerofmass);
		//std::cout << "force point " << getPointGlobalPos(point).y << "\n";
		double vrel = Vector3d::dot(n, PointVel(point));
		std::cout << "c m vel " << PointVel2(centerofmass).y << "\n";
		//std::cout << "vel " << (getPointGlobalPos(point) - old.getPointGlobalPos(point)).y / newdt << "\n";
		//std::cout << "vel2 " << PointVel2(point).y << "\n";


		vrel = PointVel2(point).y;
		std::cout << "vrel " << vrel << "\n";
		double numerator = -(1 + 1) * vrel;
		double term1 = 1 / prop.mass;
		Matrix3x3 IInv = state.R * prop.Inertia * state.R.T();
		//Matrix3x3 IInv = prop.Inertia;
		//Vector3d axis = state.R.T() * Vector3d::cross(r, n);
		double term2 = Vector3d::dot(n, Vector3d::cross(IInv * Vector3d::cross(r, n), r));
		//double term2 = Vector3d::dot(n,state.R* Vector3d::cross(prop.Inertia * axis, point- centerofmass));
		double j = numerator / (term1 + term2);
		Vector3d force = (n * j);
		std::cout << "impulse " << state.impulse.y << "\n";
		std::cout << "L " << state.AngularM.x << " " << state.AngularM.y << " " << state.AngularM.z << "\n";
		std::cout << "force " << force.y << "\n";
		state.impulse = state.impulse + force;
		std::cout << "impulse after " << state.impulse.y << "\n";
		state.AngularM = state.AngularM - state.R.T() * Vector3d::cross(r, force);
		std::cout << "L after " << state.AngularM.x <<" " << state.AngularM.y << " " << state.AngularM.z << "\n";
		vrel = Vector3d::dot(n, PointVel2(point));
		std::cout << "vrel after" << vrel << "\n";
		std::cout << "c m vel after " << PointVel2(centerofmass).y << "\n";
		std::cout << getPointGlobalPos( lowestPoint()).y<<"\n";

	}
}
