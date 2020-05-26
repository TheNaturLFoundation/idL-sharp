import turtle

def __traduire_couleur(nom):
    if nom == "rouge":
        return "red"
    elif nom == "vert":
        return "green"
    elif nom == "bleu":
        return "blue"
    elif nom == "jaune":
        return "yellow"
    elif nom == "rose":
        return "pink"
    elif nom == "violet":
        return "purple"
    elif nom == "marron":
        return "saddle brown"
    elif nom == "saumon":
        return "salmon"
    elif nom == "cyan":
        return "cyan"
    elif nom == "orange":
        return "orange"
    elif nom == "orange naturl":
        return "#ed9700"
    elif nom == "or":
        return "gold"
    elif nom == "bordeaux":
        return "maroon"
    elif nom == "gris":
        return "gray"
    elif nom == "blanc":
        return "white"
    elif nom == "noir":
        return "black"
    else:
        return nom


def definir_couleur(couleur):
    turtle.color(__traduire_couleur(couleur))


def definir_epaisseur(epaisseur):
    turtle.width(epaisseur)


def couleur_de_fond(couleur):
    turtle.bgcolor(__traduire_couleur(couleur))


def fond(couleur):
    couleur_de_fond(couleur)
    turtle.clear()


def commencer_a_remplir():
    turtle.begin_fill()


def finir_de_remplir():
    turtle.end_fill()
