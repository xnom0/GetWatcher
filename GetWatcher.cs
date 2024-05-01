using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Win32;


public class GetWatcher
{
    //On import les DLL pour Gérer le status de la fenetre CLI (HIDE or SHOW)
    [DllImport("kernel32.dll")]
	  static extern IntPtr GetConsoleWindow();
	  [DllImport("user32.dll")]
	  static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	  const int SW_HIDE = 0;
   
    public static void Main(string[] args)
    {
        // On Cache la fenetre graphique de l'emulateur de terminal
	    ShowWindow(GetConsoleWindow(), SW_HIDE);

        // On verifie si l'argument -i ou -u est ajouté afin d'installer ou désinstaller GetWatcher ,sinon on execute simplement GetWatcher
        foreach (string arg in args)
        {
            if (arg == "-i") {Install();} 
            if (arg == "-h") {Help();}
            if (arg == "-u") {Uninstall();}
        }

        // Vérifie l'existence de la clé de registre
        string check_keyPath = @"SOFTWARE\GetWatcher";
        string check_path_key = "Path";

        if (Registry.CurrentUser.OpenSubKey(check_keyPath) != null)
        {          
            // Vérifie l'existence de la valeur dans la clé de registre
            RegistryKey check_key = Registry.CurrentUser.OpenSubKey(check_keyPath);
            if (check_key.GetValue(check_path_key) == null)
            {
                Console.WriteLine("[X] Error GetWatcher is not installed.");
                Console.WriteLine("Try GetWatcher.exe -h");
                System.Environment.Exit(0);
            }
        }
        else
        {
            Console.WriteLine("[X] Error GetWatcher is not installed.");
            Console.WriteLine("Try GetWatcher.exe -h");
            System.Environment.Exit(0);
        }

        // lecture des clé de registre pour récuperai les variables
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\GetWatcher");
          
        // Lit les valeurs de la clé
        string path_watcher = key.GetValue("Path").ToString();

        // Ferme la clé de registre
        key.Close();

         // Nom du journal ETW
        string etwLogName = "GetWatcher";

        // Créer un nouvel événement ETW
	    try {
            using (EventWaitHandle eventHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "Global\\GetWatcher"))
            {
                // Vérifiez si le journal existe
                if (!EventLog.Exists(etwLogName))
                {
                    // Créer le journal s'il n'existe pas
                    EventLog.CreateEventSource(etwLogName, etwLogName);
                    Console.WriteLine($"Journal ETW '{etwLogName}' créé.");
                }
                else
                {
                    Console.WriteLine($"Le journal ETW '{etwLogName}' existe déjà.");
                }
                // Attendre 1 seconde avant d'écrire l'événement
                Thread.Sleep(1000);
            }
        }
	    catch {Console.WriteLine("EventLogName Exists.");}

        // Spécifiez le répertoire à surveiller
        string directoryPath = path_watcher;

        // Créez une instance de FileSystemWatcher
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = directoryPath;

        // Activez les notifications pour la création, la suppression, la modification et le renommage de fichiers et de dossiers
        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Attributes;

        // Activez la surveillance récursive
        watcher.IncludeSubdirectories = true;

        // Ajoutez des gestionnaires d'événements pour les événements que vous souhaitez surveiller
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Changed += OnChanged;
        watcher.Renamed += OnRenamed;

        // Démarrez la surveillance
        watcher.EnableRaisingEvents = true;
        
        // Arreter la surveillance
        //watcher.EnableRaisingEvents = false;

        Console.ReadKey();
    }

    // Méthode appelée lorsque un fichier ou un dossier est créé
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        // Nom du journal ETW
        string etwLogName = "GetWatcher";
        try 
        {
            // Écrire un événement dans le journal ETW
            using (EventLog eventLog = new EventLog(etwLogName))
            {
                eventLog.Source = etwLogName;
                // Initialiser la liste noire des extensions
                HashSet<string> blacklistedExtensions = new HashSet<string> { ".automaticDestinations-ms", ".TMP"  };
                // Demander à l'utilisateur d'entrer le nom d'un fichier avec son extension
                string fileName = e.FullPath;
                // Extraire l'extension du fichier
                string fileExtension = System.IO.Path.GetExtension(fileName);
                // Vérifier si l'extension est dans la liste noire
                if (blacklistedExtensions.Contains(fileExtension))
                {
                   Console.WriteLine("Accès refusé. Ce type de fichier est interdit.");
                }
                else
                {
                    eventLog.WriteEntry($"Created : {e.FullPath}", EventLogEntryType.Information);
                }
            }
        } 
        catch
        {
            Console.WriteLine("Error : unable to create a log");
        }
    }

    // Méthode appelée lorsque un fichier ou un dossier est supprimé
    private static void OnDeleted(object sender, FileSystemEventArgs e)
    {
        // Nom du journal ETW
        string etwLogName = "GetWatcher";
        try 
        {
            // Écrire un événement dans le journal ETW
            using (EventLog eventLog = new EventLog(etwLogName))
            {
                eventLog.Source = etwLogName;
                // Initialiser la liste noire des extensions
                HashSet<string> blacklistedExtensions = new HashSet<string> { ".automaticDestinations-ms", ".TMP" };
                // Demander à l'utilisateur d'entrer le nom d'un fichier avec son extension
                string fileName = e.FullPath;
                // Extraire l'extension du fichier
                string fileExtension = System.IO.Path.GetExtension(fileName);
                // Vérifier si l'extension est dans la liste noire
                if (blacklistedExtensions.Contains(fileExtension))
                {
                    Console.WriteLine("Accès refusé. Ce type de fichier est interdit.");
                }
                else
                {
                    eventLog.WriteEntry($"Deleted : {e.FullPath}", EventLogEntryType.Information);
                }
            }
        } 
        catch
        {
            Console.WriteLine("Error : unable to create a log");
        }
    }

    // Méthode appelée lorsque un fichier ou un dossier est modifié
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        // Nom du journal ETW
        string etwLogName = "GetWatcher";
        try 
        {
            // Écrire un événement dans le journal ETW
            using (EventLog eventLog = new EventLog(etwLogName))
            {
                eventLog.Source = etwLogName;
                // Initialiser la liste noire des extensions
                HashSet<string> blacklistedExtensions = new HashSet<string> { ".automaticDestinations-ms", ".TMP" };
                // Demander à l'utilisateur d'entrer le nom d'un fichier avec son extension
                string fileName = e.FullPath;
                // Extraire l'extension du fichier
                string fileExtension = System.IO.Path.GetExtension(fileName);
                // Vérifier si l'extension est dans la liste noire
                if (blacklistedExtensions.Contains(fileExtension))
                {
                    Console.WriteLine("Accès refusé. Ce type de fichier est interdit.");
                }
                else
                {
                    eventLog.WriteEntry($"Modified : {e.FullPath}", EventLogEntryType.Information);
                }
            }
        } 
        catch
        {
            Console.WriteLine("Error : unable to create a log");
        }
    }

    // Méthode appelée lorsque un fichier ou un dossier est renommé
    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        // Nom du journal ETW
        string etwLogName = "GetWatcher";
         try 
        {
            // Écrire un événement dans le journal ETW
            using (EventLog eventLog = new EventLog(etwLogName))
            {
                eventLog.Source = etwLogName;
                // Initialiser la liste noire des extensions
                HashSet<string> blacklistedExtensions = new HashSet<string> { ".automaticDestinations-ms", ".TMP" };
                // Demander à l'utilisateur d'entrer le nom d'un fichier avec son extension
                string fileName = e.FullPath;
                // Extraire l'extension du fichier
                string fileExtension = System.IO.Path.GetExtension(fileName);
                // Vérifier si l'extension est dans la liste noire
                if (blacklistedExtensions.Contains(fileExtension))
                {
                    Console.WriteLine("Accès refusé. Ce type de fichier est interdit.");
                }
                else
                {
                    eventLog.WriteEntry($"Rename : {e.OldName} -> {e.FullPath}", EventLogEntryType.Information);
                }
            }
        } 
        catch
        {
            Console.WriteLine("Error : unable to create a log");
        }
    }

    //Install GetWatcher
     public static void Install()
    {
        try {
            // Chemin du fichier exécutable en cours d'exécution
            string sourcePath_install = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // Chemin de destination pour la copie
            string destinationPath_install = @"C:\Windows\System32\GetWatcher.exe";
            try
                {
                // Copier le fichier exécutable
                File.Copy(sourcePath_install, destinationPath_install, true);
                Console.WriteLine("[+] Fichier copié avec succès.");
                try
                    {
                    // Ouvrir la clé de registre
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                    // Vérifier si la clé existe
                    if (key != null)
                    {
                        // Modifier la valeur de la clé UserInit
                        key.SetValue("Userinit", "C:\\Windows\\system32\\userinit.exe, C:\\Windows\\System32\\GetWatcher.exe", RegistryValueKind.String);
                        Console.WriteLine("[+] La clé de registre UserInit a été modifiée avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("[X] La clé de registre UserInit n'a pas été trouvée.");
                    }

                    // Crée une clé de registre
                    RegistryKey key1 = Registry.CurrentUser.CreateSubKey(@"Software\GetWatcher");
                    // Écrit une valeur dans la clé
                    key1.SetValue("Path", @"C:\Users");
                    // Fermer la clé de registre
                    key1.Close();
                    // Fermer la clé de registre
                    key.Close();

                   //Terminée
                   Console.WriteLine("[+] GetWatcher installed successfully a restart is necessary"); 
                   System.Environment.Exit(0);
                    }
                    catch (Exception ex) { Console.WriteLine($"[X] Une erreur s'est produite : {ex.Message}"); }
                }
                catch (IOException e) { Console.WriteLine($"[X] Une erreur s'est produite lors de la copie du fichier : {e.Message}"); }

        } catch {Console.WriteLine("[X] Error Install.GetWatcher..");System.Environment.Exit(0);}
    }



    //Désinstall l'outil GetWatcher
    public static void Uninstall()
    {
        // Chemin de la clé de registre à supprimer
        try
        {
            // Supprime la clé de registre
            Registry.CurrentUser.DeleteSubKeyTree("Software\\GetWatcher");
            try
                {
                    // Ouvrir la clé de registre
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                    // Vérifier si la clé existe
                    if (key != null)
                    {
                        // Modifier la valeur de la clé UserInit
                        key.SetValue("Userinit", "C:\\Windows\\system32\\userinit.exe,", RegistryValueKind.String);
                        Console.WriteLine("[+] La clé de registre UserInit a été modifiée avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("[X] La clé de registre UserInit n'a pas été trouvée.");
                    } }
                    catch (Exception ex) { Console.WriteLine($"[X] Une erreur s'est produite : {ex.Message}"); }


            Console.WriteLine("[+] GetWatcher uninstalled with successfully");
            System.Environment.Exit(0);
        }
        catch
        {
            Console.WriteLine("[X] Error Install.GetWatcher..");System.Environment.Exit(0);
        }
    }

      //Menu Help
     public static void Help()
    {
        Console.WriteLine("\nGetWatcher, log your actions. ");
        Console.WriteLine("2019 - 2024 Tristan Manzano\n");
        Console.WriteLine("Example : GetWatcher [-i] [-h] [-u]\n");
        Console.WriteLine("Function : \n");
        Console.WriteLine("-i : Install GetWatcher on your system");
        Console.WriteLine("-u : Uninstall GetWatcher on your system");
        Console.WriteLine("-h : Show help menu\n");
        System.Environment.Exit(0);
    }
}
