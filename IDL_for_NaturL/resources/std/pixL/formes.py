from . import style
from .geometrie import *
import turtle

def point(position):
    init_geometrie()
    turtle.goto(position.x, position.y)
    turtle.dot()


def ligne(position1, position2):
    init_geometrie()
    turtle.goto(position1.x, position1.y)
    turtle.down()
    turtle.goto(position2.x, position2.y)


def rectangle(position, largeur, hauteur):
    init_geometrie()
    turtle.goto(position.x, position.y)
    turtle.down()
    for i in range(1, 5):
        if i % 2 == 0:
            cote = hauteur
        else:
            cote = largeur
        turtle.forward(cote)
        turtle.left(90)


def rectangle_plein(position, largeur, hauteur):
    init_geometrie()
    turtle.goto(position.x - largeur / 2, position.y - hauteur / 2)
    turtle.down()
    style.commencer_a_remplir()
    for i in range(1, 5):
        if i % 2 == 0:
            cote = hauteur
        else:
            cote = largeur
        turtle.forward(cote)
        turtle.left(90)
    style.finir_de_remplir()


def cercle(position, rayon):
    init_geometrie()
    turtle.goto(position.x, position.y - rayon)
    turtle.down()
    turtle.circle(rayon)


def cercle_plein(position, rayon):
    init_geometrie()
    turtle.goto(position.x, position.y - rayon)
    turtle.down()
    style.commencer_a_remplir()
    turtle.circle(rayon)
    style.finir_de_remplir()
