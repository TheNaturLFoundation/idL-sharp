import turtle

def __traduire_touche(touche):
    if touche == "haut":
        return "Up"
    elif touche == "bas":
        return "Down"
    elif touche == "droite":
        return "Right"
    elif touche == "gauche":
        return "Left"
    elif touche == "effacer":
        return "BackSpace"
    elif (touche == "controle") or (touche == "ctrl"):
        return "Control_L"
    elif touche == "supprimer":
        return "Delete"
    elif touche == "fin":
        return "End"
    elif (touche == "entrer") or (touche == "retourner"):
        return "Return"
    else:
        return touche


def detecter_clic_gauche(action):
    turtle.onscreenclick(action, 1)


def detecter_clic_droit(action):
    turtle.onscreenclick(action, 3)


def detecter_clic_molette(action):
    turtle.onscreenclick(action, 2)


def executer_apres(action, temps):
    turtle.ontimer(action, temps)


def detecter_touche_pressee(action, touche):
    turtle.listen()
    turtle.onkeypress(action, __traduire_touche(touche))


def detecter_touche_levee(action, touche):
    turtle.listen()
    turtle.onkeyrelease(action, __traduire_touche(touche))


def lancer_partie(principale, temps):
    def _principale():
        principale()
        executer_apres(_principale, temps)


    executer_apres(_principale, temps)
    turtle.tracer(False); turtle.mainloop()
