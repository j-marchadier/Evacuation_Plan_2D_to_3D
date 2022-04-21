# Reconstruction 3D de plans d’évacuation 2D
## Information
Tous les bâtiments possèdent des plans d'évacuation. Ce sont des plans important pour savoir ou aller en cas de danger, autant pour les victime pour en sortir, que pour les urgences pour y entrer et y naviguer. \
Cependant, beaucoup de plans de ce type sont incompréhensibles, peu lisibles, trop chargés, ou ont d'autres problèmes. Il n'y a aucun standard de planification, que ce soit de la représentation des murs, portes et fenêtres, aux logos de sorties de secours et extincteurs. Cela rend les bâtiments difficies à naviguer pour la premère fois quand besoin est. \
\
Ainsi, une possibilité pour rendre cette navigation plus simple est de créer un modèle 3D à partie du plan 2D dans lequel on pourrait naviguer en Réalité Virtuelle, ou simplement l'observer. \
La modélisation virtuelle de plans 2D se fait aujourd'hui, mais seulement manuellement, ce qui prend beaucoup de temps, jusqu'a des semaines de travail de modélisation pour un seul étage de bâtiment. \
\
Notre projet vise à rendre cette modélisation, ce passage de la 2D à la 3D, simple, rapide, et efficace.
## Traitement d'image
### Présentation
Cette partie se passe en plusieurs étapes. On commence par sélectionner une carte 2D complète comprenant sa légende. \
Une fois cette carte sélectionnée, un programme va définit quand quelle zone de l'image la légende est présente, et où est le plan lui-même.
* Une IHM est ouverte pour permettre à l'utilisateur de vérifier les résultats du traitement et les ajuster si besoin. 

Les deux zones sont enregistrés en temps qu'images et traitées à leur tour. \
La légender est passée dans un nouveau programme où les différents logos sont capturés et enregistrés afin de les reconnaîte plus tard.

* A la fin du traitement, une IHM est ouverte pour permettre à l'utilisateur de vérifier les résultats du traitement et les ajuster si besoin.

Une fois les logos délimités, ils sont enregistrés dans une base de donnée. \
Le programme passe à présent sur l'image contenant simplement le plan enregistré précédemment, et va retirer au mieux toutes les itérations des logos retrouvées sur l'image. L'image traitée est enregistrée. \
Cette image est passée en niveaux de gris, afin d'avoir une grande délimitation d'intensité entre les murs et le sol, ce qui nous permet d'utiliser la différence (gradient) pour retrouver les lignes présentes sur le plan, et récupérer leurs coordonnées de début et de fin chacune. \
Un fichier d'extension `.txt` contenant cette information est produit à ce point.

### Lancement du programme
* Lancer le traitement d'image va lancer toute la chaines de programmes du début à la fin, mais il est possible de lancer chaque programme indépendamment.
* Pour lancer le programme général : ------


## Modélisation 3D Unity
### Présentation
L'exécutable produit sur Unity3D permet de lire des fichier spéciaux d'extensions `.txt` pour produire des modélisations 3D correspondantes, de les ajuster au besoin, puis de les enregistrer en temps que préfabriqués. Les préfabriqués enregistrés peuvent être visualisés avec toutes les textures correspondantes afin de vérifier le travail effectué sur le programme.
* Un problème connu est que la caméra ne marche pas bien pendant la visualisation des préfabriqués s'ils ont été créés pendant la même session d'utilisation du programme. Le problème est réglé si le programme est fermé et ouvert à nouveau.

### Attention!
* Au lancement du programme `WallMake`, si le dossier `data/` n'existe pas dans la position `/../../` à partir de l'exécutable, le programme va se fermer immédiatement après l'avoir créé.
* Pour avoir la texture du sol affichée, il faut que l'image au format .jpg soir présente dans le dossier `data/plans/`, avec le nom `plan.jpg`.
* Les préfabriqués générés par le programme de modélisation 3D WallMake sont enregistrés dans le dossier output (par défaut `data/`) avec le nom `prefab_<tag>.txt`
* Les fichiers d'entré lus par le programme WallMake doivent être présents dans le dossier input (par défaut `data/`) avec le nom `<tag>_mur.txt` \
Exemples : \
fichier d'input : `1_mur.txt` \
fichier d'output : `prefab_1.txt`

### Lancement du programme
* Pour lancer le programme `WallMake`, la méthode dépend de si vous travaillez sur windows ou sur mac.
    * Pour windows, allez dans le dossier `wallmaker_windows/` pour lancer l'exécutable WallMaker.exe.
    * Pour Mac, allez dans le dossier `wallmaker_macos.app/` pour faire quelque chose.

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

### Le projet Unity
* Le projet Unity 3D est présent dans l'archive sous le nom `WallMake.zip`. Une fois décompressé, il peut être importé dans le Unity Hub (`Open -> Add project from disk`) pour être ouvert sous l'éditeur Unity.
    * Ce projet n'est pas fait pour être lancé sous l'éditeur Unity. Il a été conçu pour produire un exécutable.
    * Le projet compilera si : \
    \
    La scene comporte une source de lumière, une caméra avec le tag `MainCamera`, et un objet vide possédant le script `Base_script`. \
    \
    La scène en question est ajoutée dans les build settings.\
    \
    Les matériaux pour les murs et le plafond sont présent dans `Assets/Resources/Materials/` sous les noms `roof_material.mat` et `wall_material.mat` (présents par défaut).
    * Une différente méthode de compilation est nécessaire, dépendant de si le programme doit tourner sous windows, mac, ou linux, et le programme à été conçu pour supporter la compilation sous windows et mac.
* Les fichiers et dossiers présents dans le dossier `Assets/` du projet Unity ont été récupérés et copiés dans un dossier à part : `Unity_visu_3D/`. Ils sont déjà tous présents dans le `.zip` du projet.
    * Ce dossier contient aussi des exemples de fichiers `mur.txt` à utiliser avec l'exécutable WallMake.

## Anciens
### Modélisation 3D Python
* Les anciens fichiers sont présents dans `historique/python_3D/`.
* Le fichier `make_mesh_from_txt.py` produit une visualisation 3D d'un fichier d'entrée.
\
-> `python make_mesh_from_txt.py <fichier entree> <nom sortie>`

* Exemple : \
-> `python make_mesh_from_txt.py result.txt visu3D`
    * le fichier visu3D est maintenant une représentation 3D du fichier d'entrée. Par défaut, le nom de sortie est `mesh_with_colorv2.obj`.


