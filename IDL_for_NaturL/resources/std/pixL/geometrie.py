import turtle

class Vecteur:
    def __init__(self, x, y):
        self.x = x
        self.y = y
        



def init_geometrie():
    turtle.up()
    turtle.home()
    turtle.speed("fastest")
    turtle.delay(0)
    turtle.ht()


def largeur_fenetre():
    return turtle.window_width()


def hauteur_fenetre():
    return turtle.window_height()


def redimensionner_fenetre(largeur, hauteur):
    turtle.setup(largeur, hauteur)
