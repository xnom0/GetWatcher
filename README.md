# GetWatcher

### GetWatcher est un outil qui permet de créer un journal d'événements pour Windows (ETW) est de journaliser l'ensemble des actions effectué par les utilisateurs du système.


https://github.com/xnom0/GetWatcher/assets/168633454/88da5f0b-2d14-475f-bca9-b20d585af59a


Il permet de créer un log lorsque :
* un fichier où dossier est créé
* un fichier où dossier est modifié
* un fichier où dossier est supprimé
* un fichier où dossier est renommé

Afin de compiler GetWatcher vous pouvez utiliser mono sous GNU/Linux ou visual studio sous Windows

`mcs -out:GetWatcher.exe GetWatcher.cs`

Par défaut GetWatcher log toutes les actions effectuées dans le dossier **C:\Users\** il est possible de modifier ce répertoire dans la clé de registre suivante : ** Software\GetWatcher\Path **

Une version compilée est disponible mais je vous invite à lire le code et le compiler vous-même
