## SpaceWar

SpaceWar est un jeu arcade de type 1v1 en split-screen, où deux joueurs s'affrontent dans une arène spatiale. Le but du jeu est de survivre tout en affrontant l'autre joueur en utilisant des projectiles et des boosts de vitesse. Le jeu permet à chaque joueur de contrôler son propre vaisseau, avec des contrôles distincts pour chacun, et propose un mode de jeu rapide où la stratégie et la réactivité sont essentielles.

### Technologies Utilisées
- **Monogame**: Pour la gestion du rendu graphique, de l'animation et des entrées.
- **C#**: Langage de programmation principal utilisé pour développer la logique.
- **Microsoft Visual Studio Code**: IDE utilisé pour développer.
- **SoundEffect**: Pour la gestion des effets sonores (comme les tirs de laser).

### Fonctionnalités majeurs

1. **Mécanique de jeu**:

- **Mode 1v1 en split-screen**: Deux joueurs peuvent s'affronter sur le même écran, chacun contrôlant un vaisseau dans une arène.
- **Mouvement et rotation**: Chaque joueur peut se déplacer et tourner librement dans l’arène.
- **Système de tir**: Chaque joueur peut tirer des projectiles pour endommager l'autre. Le joueur doit gérer son nombre de munitions et utiliser les tirs de manière stratégique..
- **Boost de vitesse**: Les joueurs peuvent activer un boost de vitesse temporaire (par défaut) pour augmenter leurs capacités de déplacement, mais cela consomme une barre de boost limitée qui se régénère lentement.
- **Gestion de la santé**: Chaque joueur a une barre de santé. Si elle atteint zéro, le joueur est éliminé. La santé se régénère automatiquement à intervalles réguliers.

2. **Système de score et conditions de victoire**:

- **Victoire**: Le but du jeu est de battre l'adversaire en lui infligeant suffisamment de dégâts pour réduire sa santé à zéro.

- **Calcul du score**: Le score final est calculé en fonction de ces facteurs, avec un soft cap pour que les joueurs ne puissent pas accumuler un score excessivement élevé juste en fonction de la durée du match. Voici le calcul du score :
    - **Précision**: Ce score est basé sur le ratio des tirs réussis par rapport au nombre total de tirs effectués.
    - **Agression**: Ce score récompense les joueurs qui infligent des dégâts, avec un effet de diminution exponentielle pour éviter un score trop élevé basé uniquement sur les dégâts.
    - **Esquives**: Le score d'esquive est calculé en fonction du nombre d'esquives effectuées.
    - **Mobilité**: Plus un joueur est en mouvement, plus il est récompensé par ce score.
    - **Portée moyenne des tirs**: Ce score prend en compte la distance moyenne des tirs effectués par un joueur pour récompenser la précision à longue portée.
    - **Bonus de victoire**: Un bonus fixe est accordé au joueur gagnant du match.

### Structure Algorithmique

1. **Contrôle du joueur**:
Chaque joueur utilise des touches dédiées pour se déplacer (haut, bas, gauche, droite), tourner et tirer.

Les déplacements des vaisseaux sont contrôlés par des vecteurs, et la rotation du vaisseau est gérée avec des changements d'angle (en fonction des touches directionnelles).

2. **Gestion des projectiles**:
Lorsqu'un joueur appuie sur la touche de tir, un projectile est créé à la position de son vaisseau et se déplace dans la direction choisie par la rotation du vaisseau.

Les projectiles sont supprimés lorsqu'ils sortent de l'écran ou lorsqu'ils entrent en collision avec un autre vaisseau ou projectile.

3. **Boost de vitesse**:
Le boost est activé lorsque la touche de boost est pressée. Cela augmente temporairement la vitesse du joueur, mais consomme une barre de boost. Le boost se régénère après un certain délai.

4. **Gestion de la santé et de la régénération**:
La santé des joueurs se régénère lentement au fil du temps, à condition que la santé ne soit pas déjà pleine.

Lorsqu'un joueur est touché par un projectile, il subit des dégâts et sa barre de santé diminue. Un effet visuel (clignotement) indique que le joueur a pris des dégâts.

### Installation
**Prérequis**
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [MonoGame 3.8.1 ou supérieur](https://www.monogame.net/downloads/)

**Étapes d'Installation**
1. Cloner le dépôt :

```bash
git clone https://github.com/DallasJr/ProjetDev.git
cd ./ProjetDev/SpaceWar
```

2. Installation des outils MonoGame (si ce n'est pas déjà fait)
Vérifie que l’outil est bien installé :
```bash
dotnet mgcb --version
```
sinon
```bash
dotnet tool install --global dotnet-mgcb
```

3. Restauration des dépendances
```bash
dotnet restore
```

4. Compilation et lancement du jeu
```bash
dotnet build
dotnet run --project SpaceWar
```

Projet Dev - YNOV CAMPUS Paris

Développeurs: Dallas, Clément
