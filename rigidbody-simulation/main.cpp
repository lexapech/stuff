#include <SFML/Window.hpp>
#include <SFML/OpenGL.hpp>
#include <GL/gl.h>
#include <GL/glu.h>
#include "func.h"

#include <iostream>




int main()
{
    double maxheight=0;

    // create the window
    sf::Window window(sf::VideoMode(800, 600), "OpenGL", sf::Style::Default, sf::ContextSettings(32));
    
    window.setVerticalSyncEnabled(true);

    // activate the window
    window.setActive(true);

    // load resources, initialize the OpenGL states, ...
    glClearColor(0.0f, 0.0f, 0.0f, 0.0f); // устанавливаем фоновый цвет на черный
    glClearDepth(1.0);
    glDepthFunc(GL_LESS);
    glEnable(GL_DEPTH_TEST); // включаем тест глубины
    glShadeModel(GL_SMOOTH);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(45.0f, (float)800 / (float)600, 0.1f, 100.0f); // настраиваем трехмерную перспективу
    glMatrixMode(GL_MODELVIEW);
    // run the main loop
    bool running = true;
    RigidBody rb({ 3,1,1 });
    
    rb.prop.Inertia = Matrix3x3();
    rb.prop.forcePoint=Vector3d(0, 0, 0);
    rb.prop.forceDir= Vector3d(0, -9.81*rb.prop.mass, 0);
    //rb.prop.forceDir = Vector3d(0, 0, 0);
    //rb.prop.Inertia = Matrix3x3::Inv(rb.CalcInertia());
    rb.prop.Inertia = Matrix3x3::InvDiag(rb.CalcInertia());
    double a = 2 * 3.14 /180;
    double b = 75 * 3.14 / 180;
    //double b = 15 * 3.14 / 180;
    Quaternion q = Quaternion(std::cos(a / 2), Vector3d(1, 0, 1) * std::sin(a / 2))* Quaternion(std::cos(b / 2), Vector3d(0, 1, 0) * std::sin(b / 2));
    State st = { Vector3d{0,10,0},Vector3d(),q,Vector3d(),Matrix3x3(q) };
    rb.state = st;
    //rb.state.AngularM = Vector3d(0,0,71);
    //rb.state.impulse.y = -200;
    sf::Clock clock;
	double t=0;
    while (running)
    {
        // handle events
        sf::Event event;
        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed)
            {
                // end the program
                running = false;
            }
            else if (event.type == sf::Event::Resized)
            {
                // adjust the viewport when the window is resized
                glViewport(0, 0, event.size.width, event.size.height);
                //gluPerspective(80.0f, (float)event.size.width / (float)event.size.height, 0.1f, 100.0f); // настраиваем трехмерную перспективу
            }
        }

        sf::Time elapsed = clock.getElapsedTime();
        
        //std::cout << "step time: " << elapsed.asSeconds() << "\n";
         t += elapsed.asSeconds();
        for (; t > 0.008; t -= 0.016)
        {
        
        rb.step(0.016);
        }
        clock.restart();
       // rb.step(0.002);
        //rb.step(0.1);


        //maxheight = std::max(maxheight, rb.state.pos.y);
        //std::cout << st.pos.y << "\n";
        // clear the buffers
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // draw...
        
        
        glPushMatrix();
        
        glTranslatef(0, -5, -20);
        glRotatef(20, 1, 0, 0);
        
        cl::Plane();
        glPushMatrix();
        glRotatef(90, 0, -1, 0);
        rb.draw();
        glPopMatrix();
        
        
        
        
        glPopMatrix();
        // end the current frame (internally swaps the front and back buffers)
        window.display();
    }

    // release resources...

    return 0;
}
