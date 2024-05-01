# GetWatcher

### GetWatcher est un outil qui permet de créer un journal d'événements pour Windows (ETW) est de journaliser l'ensemble des actions effectué par les utilisateurs du système.

![Screenshot_20240501_153854](https://github.com/xnom0/GetWatcher/assets/168633454/ef1013c4-331a-4e79-9f3e-4cec5b97dd55)

Il permet de créer un log lorsque :
* un fichier où dossier est créé
* un fichier où dossier est modifié
* un fichier où dossier est supprimé
* un fichier où dossier est renommé

Afin de compiler GetWatcher vous pouvez utiliser mono sous GNU/Linux ou visual studio sous Windows

`mcs -out:GetWatcher.exe GetWatcher.cs`

Par défaut GetWatcher log toutes les actions effectuées dans le dossier `C:\Users\` il est possible de modifier ce répertoire dans la clé de registre suivante :  **Software\GetWatcher\Path**

Une version compilée est disponible mais je vous invite à lire le code et le compiler vous-même.

le format des logs est classiques (ETW) il est donc possible de les intégrer directement à un siem 😉 

<font color='red'>*PS* Je ne suis pas développeur donc n'hésitez pas à proposer vos améliorations sur ce projet</font>

### Installation 
Exécuté GetWatcher.exe avec l'invite de commande ou powershell (les droits d'administration sont obligatoires pour l'installation), puis avec l'argument -i il est possible d'installer GetWatcher.

 `GetWatcher.exe -i`

 ### Désinstallation

 Exécuté GetWatcher.exe avec l'invite de commande ou powershell (les droits d'administration sont obligatoires pour l'installation), puis avec l'argument -u il est possible de désinstaller GetWatcher.

  `GetWatcher.exe -u`

  ## Vidéo de démonstration 
  https://github.com/xnom0/GetWatcher/assets/168633454/88da5f0b-2d14-475f-bca9-b20d585af59a
