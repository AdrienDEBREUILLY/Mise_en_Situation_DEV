﻿﻿
using System;
using System.Collections.Generic;
using System.IO;

// Structure représentant un client
public struct Client
{
    public int NumeroClient; // Identifiant unique du client
    public string Nom; // Nom de famille
    public string Prenom; // Prénom
    public string? NumeroTelephone; // Numéro de téléphone (nullable)
}

public static class Program
{
    // Nom du fichier binaire pour stocker les données des clients
    private static readonly string FichierClients = "clients.bin";

    public static void Main()
    {
        // Point d'entrée principal
        AfficherMenu();
    }

    static void AfficherMenu()
    {
        // Affiche un menu interactif en boucle
        while (true)
        {
            Console.Clear(); // Efface la console pour une meilleure lisibilité
            Console.WriteLine("1. Saisir un nouveau client");
            Console.WriteLine("2. Afficher un client");
            Console.WriteLine("3. Afficher tous les clients");
            Console.WriteLine("4. Afficher le nombre de clients");
            Console.WriteLine("5. Modifier un client");
            Console.WriteLine("6. Supprimer un client");
            Console.WriteLine("7. Restaurer une fiche supprimée");
            Console.WriteLine("8. Afficher les fiches supprimées");
            Console.WriteLine("9. Compresser le fichier");
            Console.WriteLine("10. AfficherStatistiques");
            Console.WriteLine("11. Quitter");
            Console.Write("Choix : ");

            if (int.TryParse(Console.ReadLine(), out int choix))
            {
                // Appelle la méthode correspondant au choix de l'utilisateur
                switch (choix)
                {
                    case 1:
                        AjouterClient();
                        break;
                    case 2:
                        AfficherClient();
                        break;
                    case 3:
                        AfficherTousLesClients();
                        break;
                    case 4:
                        CompterClients();
                        break;
                    case 5:
                        ModifierClient();
                        break;
                    case 6:
                        SupprimerClient();
                        break;
                    case 7:
                        RestaurerClient();
                        break;
                    case 8:
                        AfficherFichesSupprimees();
                        break;
                    case 9:
                        CompresserFichier();
                        break;
                    case 10:
                        AfficherStatistiques();
                        break;
                    case 11:
                        Console.WriteLine("Au revoir !");
                        Environment.Exit(0); // Quitte l'application
                        break;
                    default:
                        Console.WriteLine("Option invalide. Réessayez.");
                        break;
                }
            }
            else
            {
                // Message d'erreur si l'utilisateur n'entre pas un nombre valide
                Console.WriteLine("Veuillez entrer un nombre valide.");
            }

            Console.WriteLine("Appuyez sur Entrée pour continuer...");
            Console.ReadLine();
        }
    }

    private static void AjouterClient()
    {
        // Permet d'ajouter un nouveau client
        Client nouveauClient = new Client();

        Console.Write("Numéro de client : ");
        nouveauClient.NumeroClient = LireEntierUtilisateur(); // Lecture sécurisée d'un entier

        Console.Write("Nom : ");
        nouveauClient.Nom = Majuscule(Console.ReadLine()); // Formate le nom en majuscules

        Console.Write("Prénom : ");
        nouveauClient.Prenom = FirstMajuscule(Console.ReadLine()); // Majuscule initiale pour le prénom

        Console.Write("Numéro de téléphone : ");
        nouveauClient.NumeroTelephone = Console.ReadLine();

        // Chargement des clients existants
        List<Client> clients = LireTousLesClients();

        bool ficheReutilisee = false;
        for (int i = 0; i < clients.Count; i++)
        {
            // Vérifie s'il existe une fiche supprimée à réutiliser
            if (clients[i].Nom == "*")
            {
                clients[i] = nouveauClient;
                ficheReutilisee = true;
                break;
            }
        }

        if (!ficheReutilisee)
        {
            // Si aucune fiche supprimée n'est trouvée, ajoute le nouveau client
            clients.Add(nouveauClient);
        }

        // Sauvegarde la liste mise à jour dans le fichier
        EcrireTousLesClients(clients);

        Console.WriteLine("Client ajouté avec succès.");
    }

    private static void AfficherClient()
    {
        // Recherche et affiche un client par son nom
        Console.Write("Nom du client à rechercher : ");
        string nomRecherche = Majuscule(Console.ReadLine());

        List<Client> clients = LireTousLesClients(); // Charge tous les clients
        bool clientTrouve = false;

        for (int i = 0; i < clients.Count; i++)
        {
            // Recherche des fiches correspondant au nom
            if (clients[i].Nom == nomRecherche)
            {
                clientTrouve = true;
                Console.WriteLine($"Fiche {i + 1}: {clients[i].NumeroClient}, {clients[i].Nom}, {clients[i].Prenom}, {clients[i].NumeroTelephone}");
            }
        }

        if (!clientTrouve)
        {
            // Affiche un message si aucun client n'est trouvé
            Console.WriteLine("Aucun client trouvé avec ce nom.");
        }
    }

    private static void AfficherTousLesClients()
    {
        // Affiche toutes les fiches clients actives
        List<Client> clients = LireTousLesClients();
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Nom != "*") // Ignore les fiches supprimées
            {
                Console.WriteLine($"Fiche {i + 1}: {clients[i].NumeroClient}, {clients[i].Nom}, {clients[i].Prenom}, {clients[i].NumeroTelephone}");
            }
        }
    }

    private static void CompterClients()
    {
        // Compte et affiche le nombre de fiches actives
        List<Client> clients = LireTousLesClients();
        int compteur = 0;

        foreach (var client in clients)
        {
            if (client.Nom != "*") compteur++; // Compte uniquement les fiches actives
        }

        Console.WriteLine($"Nombre de clients : {compteur}");
    }

    private static void ModifierClient()
    {
        // Permet de modifier une fiche client
        Console.Write("Numéro de la fiche à modifier : ");
        int numeroFiche = LireEntierUtilisateur();

        List<Client> clients = LireTousLesClients();

        if (numeroFiche > 0 && numeroFiche <= clients.Count)
        {
            Client client = clients[numeroFiche - 1];
            if (client.Nom == "*") // Vérifie si la fiche est supprimée
            {
                Console.WriteLine("Cette fiche est supprimée.");
                return;
            }

            // Propose les modifications champ par champ
            Console.WriteLine($"Modification de la fiche : {client.NumeroClient}, {client.Nom}, {client.Prenom}, {client.NumeroTelephone}");

            Console.Write("Nouveau nom (laisser vide pour conserver) : ");
            string? nom = Console.ReadLine();
            if (!string.IsNullOrEmpty(nom)) client.Nom = Majuscule(nom);

            Console.Write("Nouveau prénom (laisser vide pour conserver) : ");
            string? prenom = Console.ReadLine();
            if (!string.IsNullOrEmpty(prenom)) client.Prenom = FirstMajuscule(prenom);

            Console.Write("Nouveau numéro de téléphone (laisser vide pour conserver) : ");
            string? numeroTelephone = Console.ReadLine();
            if (!string.IsNullOrEmpty(numeroTelephone)) client.NumeroTelephone = numeroTelephone;

            // Met à jour la fiche dans la liste
            clients[numeroFiche - 1] = client;
            EcrireTousLesClients(clients);

            Console.WriteLine("Client modifié avec succès.");
        }
        else
        {
            // Message d'erreur pour une fiche invalide
            Console.WriteLine("Fiche non trouvée.");
        }
    }

    private static void SupprimerClient()
    {
        // Permet de marquer une fiche client comme supprimée
        Console.Write("Numéro de la fiche à supprimer : ");
        int numeroFiche = LireEntierUtilisateur();

        // Charge la liste des clients depuis le fichier binaire
        List<Client> clients = LireTousLesClients();

        // Vérifie si la fiche existe
        if (numeroFiche > 0 && numeroFiche <= clients.Count)
        {
            // Accéder à la fiche spécifique et marquer la suppression
            Client clientASupprimer = clients[numeroFiche - 1];
            clientASupprimer.Nom = "*"; // Suppression logique
            clients[numeroFiche - 1] = clientASupprimer;

            // Réécrire la liste dans le fichier binaire
            EcrireTousLesClients(clients);

            Console.WriteLine($"Client numéro {numeroFiche} supprimé avec succès.");
        }
        else
        {
            // Message d'erreur pour une fiche invalide
            Console.WriteLine("Fiche non trouvée. Aucune suppression effectuée.");
        }
    }

    private static void RestaurerClient()
    {
        // Permet de restaurer une fiche supprimée
        Console.Write("Numéro de la fiche à restaurer : ");
        int numeroFiche = LireEntierUtilisateur();

        List<Client> clients = LireTousLesClients();

        if (numeroFiche > 0 && numeroFiche <= clients.Count)
        {
            Client client = clients[numeroFiche - 1];
            if (client.Nom == "*") // Vérifie si la fiche est marquée comme supprimée
            {
                Console.Write("Entrez le nouveau nom pour ce client : ");
                client.Nom = Majuscule(Console.ReadLine()); // Récupère un nouveau nom pour le restaurer

                // Mise à jour du client restauré
                clients[numeroFiche - 1] = client;

                // Réécrit la liste dans le fichier
                EcrireTousLesClients(clients);

                Console.WriteLine($"Client numéro {numeroFiche} restauré avec succès.");
            }
            else
            {
                Console.WriteLine("Cette fiche n'est pas marquée comme supprimée.");
            }
        }
        else
        {
            Console.WriteLine("Numéro de fiche invalide.");
        }
    }

    private static void AfficherFichesSupprimees()
    {
        // Affiche toutes les fiches marquées comme supprimées
        List<Client> clients = LireTousLesClients();
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Nom == "*") // Seules les fiches supprimées sont affichées
            {
                Console.WriteLine($"Fiche {i + 1}: {clients[i].NumeroClient}, {clients[i].Nom}, {clients[i].Prenom}, {clients[i].NumeroTelephone}");
            }
        }
    }

    private static void CompresserFichier()
    {
        // Supprime définitivement toutes les fiches marquées comme supprimées
        List<Client> clients = LireTousLesClients();
        List<Client> clientsExistants = clients.FindAll(client => client.Nom != "*"); // Garde uniquement les fiches actives
        EcrireTousLesClients(clientsExistants); // Réécrit uniquement les clients actifs dans le fichier

        Console.WriteLine("Fichier compressé avec succès.");
    }

    private static void AfficherStatistiques()
    {
        // Affiche des statistiques sur les fiches clients
        List<Client> clients = LireTousLesClients(); // Charge tous les clients

        int total = clients.Count; // Nombre total de fiches
        int actifs = clients.FindAll(c => c.Nom != "*").Count; // Fiches actives
        int supprimes = total - actifs; // Fiches supprimées logiquement

        // Affiche les statistiques
        Console.WriteLine("Statistiques :");
        Console.WriteLine($"- Total : {total}");
        Console.WriteLine($"- Actifs : {actifs}");
        Console.WriteLine($"- Supprimés logiquement : {supprimes}");
    }

    private static string Majuscule(string? input)
    {
        // Convertit une chaîne en majuscules
        return input?.ToUpper() ?? string.Empty;
    }

    private static string FirstMajuscule(string? input)
    {
        // Met la première lettre d'une chaîne en majuscule et le reste en minuscules
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    private static int LireEntierUtilisateur()
    {
        // Lecture sécurisée d'un entier fourni par l'utilisateur
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int result))
                return result;

            Console.WriteLine("Veuillez entrer un entier valide.");
        }
    }

    private static List<Client> LireTousLesClients()
    {
        // Charge tous les clients depuis le fichier binaire
        if (!File.Exists(FichierClients)) return new List<Client>();

        using var fs = new FileStream(FichierClients, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);
        var clients = new List<Client>();

        // Lit chaque client du fichier
        while (fs.Position < fs.Length)
        {
            var client = new Client
            {
                NumeroClient = br.ReadInt32(),
                Nom = br.ReadString(),
                Prenom = br.ReadString(),
                NumeroTelephone = br.ReadString()
            };
            clients.Add(client);
        }

        return clients;
    }

    private static void EcrireTousLesClients(List<Client> clients)
    {
        // Écrit tous les clients dans le fichier binaire
        using var fs = new FileStream(FichierClients, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);
        foreach (var client in clients)
        {
            // Écrit chaque champ d'un client dans le fichier
            bw.Write(client.NumeroClient);
            bw.Write(client.Nom);
            bw.Write(client.Prenom);
            if (client.NumeroTelephone != null) bw.Write(client.NumeroTelephone);
        }
    }
}
