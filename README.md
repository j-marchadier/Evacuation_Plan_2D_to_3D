# Evacuation_Plan_2D_to_3D
## Traitement d'image
### Présentation



## Modélisation 3D Unity
### Présentation
L'exécutable produit sur Unity3D permet de lire des fichier spéciaux d'extensions `.txt`pour produire des modélisations 3D correspondantes, de les ajuster au besoin, puis de les enregistrer en temps que préfabriqués. Les préfabriqués enregistrés peuvent être visualisés avec toutes les textures afin de vérifier le travail effectué.
* Un problème connu est que la caméra ne marche pas bien pendant la visualisation des préfabriqués s'ils ont été créés pendant la même session d'utilisation du programme. Le problème est réglé si le programme est fermé et ouvert à nouveau.

### Touches et utilités
Touche              |Fonction                                   |Limites
-------             |--------                                   |-------
P                   |passer en mode modélisation (par défaut)
V                   | passer en mode visualisation
R                   | laisser la caméra tourner automatiquement
T                   | changer la direction de rotation de la caméra
Echap               | quitter le programme
Flèches de droite   |passer au modèle suivant à droite|
Flèches de gauche   | passer au modèle suivant |
A                   | diminuer la taille des murs           |seulement en modélisation
Z                   | augmenter la taille des murs|seulement en modélisation
Q                   | diminuer l’épaisseur des murs|seulement en modélisation
S                   | augmenter l’épaisseur des murs|seulement en modélisation
W                   | diminuer la longueur du plafond et du sol|seulement en modélisation
X                   | augmenter la longueur du plafond et du sol|seulement en modélisation
H                   | cacher le plafond|seulement en modélisation
Entrée              | enregistrer un préfabriqué|seulement en modélisation


## Anciens
### Modélisation 3D Python
* Le fichier `make_mesh_from_txt.py` produit une visualisation 3D d'un fichier d'entrée.
\
-> `python make_mesh_from_txt.py <fichier entree> <nom sortie>`

* Exemple : \
-> `python make_mesh_from_txt.py result.txt visu3D`
    * le fichier visu3D est maintenant une représentation 3D du fichier d'entrée.


