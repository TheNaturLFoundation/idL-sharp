from .geometrie import *
from threading import Thread
import turtle

class Police:
    def __init__(self, nom, taille):
        self.nom = nom
        self.taille = taille
        



def ecrire_texte(position, police, texte):
    init_geometrie()
    turtle.goto(position.x, position.y)
    turtle.down()
    Thread(target=lambda: turtle.write(texte, align='center', font=(police.nom, police.taille, 'normal'))).start()  # This action takes too long so it is executed in a thread
