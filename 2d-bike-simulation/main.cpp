#include "main.h"

int getKeys()
{
    if (sf::Keyboard::isKeyPressed(sf::Keyboard::Left))
        return -1;
    if (sf::Keyboard::isKeyPressed(sf::Keyboard::Right))
        return 1;
    if (sf::Keyboard::isKeyPressed(sf::Keyboard::Down))
        return 0;
    return 2;
}


int main()
{
    sf::RenderWindow window(sf::VideoMode(1000, 600), "bike");

    Bike b;

    sf::View w;
    w=window.getDefaultView();
    w.setCenter(0, 0);
    window.setView(w);

    sf::Sprite bike;
    sf::Texture t;
    t.loadFromFile("imgs/bike1.png");
    bike.setTexture(t);
    bike.setOrigin(bike.getTexture()->getSize().x / 2, bike.getTexture()->getSize().y / 2);
    bike.setPosition(0,0);
    bike.setScale({ 2,2 });

    sf::Sprite wheel;
    sf::Texture wheelt;
    wheelt.loadFromFile("imgs/wheel.png");
    wheel.setTexture(wheelt);
    wheel.setOrigin(wheel.getTexture()->getSize().x / 2, wheel.getTexture()->getSize().y / 2);
    wheel.setPosition(0, 0);
    


    sf::RectangleShape line(sf::Vector2f(1000, 5));
    line.setFillColor(sf::Color::Black);
    line.setOrigin(line.getSize().x / 2, line.getSize().y / 2);
    line.setPosition(0, 40);


    while (window.isOpen())
    {
        sf::Event event;
        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed)
                window.close();
        }



        b.update(getKeys());
        bike.setPosition(b.b.pos.x, -b.b.pos.y);
        bike.setRotation(b.b.angle / M_PI * 180);
        window.clear(sf::Color::White);

        

        window.draw(bike);

        sf::RectangleShape springr(sf::Vector2f(b.rear.s.len(), 5));
        springr.setFillColor(sf::Color::Red);
        springr.setOrigin(springr.getSize().x / 2, springr.getSize().y / 2);
        springr.setPosition(b.rear.s.mid().x, -b.rear.s.mid().y);
        springr.setRotation(b.rear.s.angle());
        window.draw(springr);

        sf::RectangleShape springf(sf::Vector2f(b.front.s.len(), 5));
        springf.setFillColor(sf::Color::Red);
        springf.setOrigin(springf.getSize().x / 2, springf.getSize().y / 2);
        springf.setPosition(b.front.s.mid().x, -b.front.s.mid().y);
        springf.setRotation(b.front.s.angle());
        window.draw(springf);

        double scale = 2 * b.rear.radius / (double)wheelt.getSize().x;
        wheel.setScale(scale, scale);
        wheel.setPosition(b.rear.b.pos.x, -b.rear.b.pos.y);
        wheel.setRotation(b.rear.b.angle / M_PI * 180);
        window.draw(wheel);
        scale = 2 * b.front.radius / (double)wheelt.getSize().x;
        wheel.setScale(scale, scale);
        wheel.setPosition(b.front.b.pos.x, -b.front.b.pos.y);
        wheel.setRotation(b.front.b.angle / M_PI * 180);
        window.draw(wheel);
        window.draw(line);
        window.display();
    }

    return 0;
}