using System;
using System.Collections.Generic;
using System.IO;

public struct Client
{
    public int NumeroClient;
    public string Nom;
    public string Prenom;
    public string? NumeroTelephone;
}

public static class Program
{
    private static readonly string FichierClients = "clients.bin";

    public static void Main()
    {
        AfficherMenu();
    }

    static void AfficherMenu()
    {
        while (true)
        {
            Console.Clear();
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
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Option invalide. Réessayez.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Veuillez entrer un nombre valide.");
            }

            Console.WriteLine("Appuyez sur Entrée pour continuer...");
            Console.ReadLine();
        }
    }

    private static void AjouterClient()
    {
        Client nouveauClient = new Client();

        Console.Write("Numéro de client : ");
        nouveauClient.NumeroClient = LireEntierUtilisateur();

        Console.Write("Nom : ");
        nouveauClient.Nom = Majuscule(Console.ReadLine());

        Console.Write("Prénom : ");
        nouveauClient.Prenom = FirstMajuscule(Console.ReadLine());

        Console.Write("Numéro de téléphone : ");
        nouveauClient.NumeroTelephone = Console.ReadLine();

        List<Client> clients = LireTousLesClients();

        bool ficheReutilisee = false;
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Nom == "*")
            {
                clients[i] = nouveauClient;
                ficheReutilisee = true;
                break;
            }
        }

        if (!ficheReutilisee)
        {
            clients.Add(nouveauClient);
        }

        EcrireTousLesClients(clients);

        Console.WriteLine("Client ajouté avec succès.");
    }

    private static void AfficherClient()
    {
        Console.Write("Nom du client à rechercher : ");
        string nomRecherche = Majuscule(Console.ReadLine());

        List<Client> clients = LireTousLesClients();
        bool clientTrouve = false;

        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Nom == nomRecherche)
            {
                clientTrouve = true;
                Console.WriteLine($"Fiche {i + 1}: {clients[i].NumeroClient}, {clients[i].Nom}, {clients[i].Prenom}, {clients[i].NumeroTelephone}");
            }
        }

        if (!clientTrouve)
        {
            Console.WriteLine("Aucun client trouvé avec ce nom.");
        }
    }

    private static void AfficherTousLesClients()
    {
        List<Client> clients = LireTousLesClients();
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Nom != "*")
            {
                Console.WriteLine($"Fiche {i + 1}: {clients[i].NumeroClient}, {clients[i].Nom}, {clients[i].Prenom}, {clients[i].NumeroTelephone}");
            }
        }
    }

    private static void CompterClients()
    {
        List<Client> clients = LireTousLesClients();
        int compteur = 0;

        foreach (var client in clients)
        {
            if (client.Nom != "*") compteur++;
        }

        Console.WriteLine($"Nombre de clients : {compteur}");
    }

    private static void ModifierClient()
    {
        Console.Write("Numéro de la fiche à modifier : ");
        int numeroFiche = LireEntierUtilisateur();

        List<Client> clients = LireTousLesClients();

        if (numeroFiche > 0 && numeroFiche <= clients.Count)
        {
            Client client = clients[numeroFiche - 1];
            if (client.Nom == "*")
            {
                Console.WriteLine("Cette fiche est supprimée.");
                return;
            }

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

            clients[numeroFiche - 1] = client;
            EcrireTousLesClients(clients);

            Console.WriteLine("Client modifié avec succès.");
        }
        else
        {
            Console.WriteLine("Fiche non trouvée.");
        }
    }

    private static void SupprimerClient()
    {
        Console.Write("Numéro de la fiche à supprimer : ");
        int numeroFiche = LireEntierUtilisateur();

        // Charger la liste des clients depuis le fichier binaire
        List<Client> clients = LireTousLesClients();

        // Vérifier si la fiche existe
        if (numeroFiche > 0 && numeroFiche <= clients.Count)
        {
            // Accéder à la fiche spécifique et marquer la suppression
            Client clientASupprimer = clients[numeroFiche - 1];
            clientASupprimer.Nom = "*"; // Suppression logique
            clients[numeroFiche - 1] = clientASupprimer;

            // Réécrire la liste dans le fichier binaire
            EcrireTousLesClients(clients);

            // Confirmation visuelle
            Console.WriteLine($"Client numéro {numeroFiche} supprimé avec succès.");
        }
        else
        {
            // Si le numéro de fiche est invalide
            Console.WriteLine("Fiche non trouvée. Aucune suppression effectuée.");
        }
    }

    private static void RestaurerClient()
    {
        Console.Write("Numéro de la fiche à restaurer : ");
        int numeroFiche = LireEntierUtilisateur();

        List<Client> clients = LireTousLesClients();

        if (numeroFiche > 0 && numeroFiche <= clients.Count)
        {
            Client client = clients[numeroFiche - 1];
            if (client.Nom == "*") // Vérifie que le client est supprimé logiquement
            {
                Console.Write("Entrez le nouveau nom pour ce client : ");
                client.Nom = Majuscule(Console.ReadLine());

                // Mise à jour du client dans la liste
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
        List<Client> clients = LireTousLesClients();
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Nom == "*")
            {
                Console.WriteLine($"Fiche {i + 1}: {clients[i].NumeroClient}, {clients[i].Nom}, {clients[i].Prenom}, {clients[i].NumeroTelephone}");
            }
        }
    }

    private static void CompresserFichier()
    {
        List<Client> clients = LireTousLesClients();
        List<Client> clientsExistants = clients.FindAll(client => client.Nom != "*");
        EcrireTousLesClients(clientsExistants);
        Console.WriteLine("Fichier compressé avec succès.");
    }

    private static void AfficherStatistiques()
    {
        List<Client> clients = LireTousLesClients(); // Charge tous les clients

        int total = clients.Count; // Nombre total de fiches
        int actifs = clients.FindAll(c => c.Nom != "*").Count; // Fiches actives
        int supprimes = total - actifs; // Fiches supprimées logiquement

        // Affiche les statistiques
        Console.WriteLine($"Statistiques :");
        Console.WriteLine($"- Total : {total}");
        Console.WriteLine($"- Actifs : {actifs}");
        Console.WriteLine($"- Supprimés logiquement : {supprimes}");
    }


    private static string Majuscule(string? input)
    {
        return input?.ToUpper() ?? string.Empty;
    }

    private static string FirstMajuscule(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    private static int LireEntierUtilisateur()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int result))
                return result;

            Console.WriteLine("Veuillez entrer un entier valide.");
        }
    }

    private static List<Client> LireTousLesClients()
    {
        if (!File.Exists(FichierClients)) return new List<Client>();

        using var fs = new FileStream(FichierClients, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);
        var clients = new List<Client>();

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
        using var fs = new FileStream(FichierClients, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);
        foreach (var client in clients)
        {
            bw.Write(client.NumeroClient);
            bw.Write(client.Nom);
            bw.Write(client.Prenom);
            if (client.NumeroTelephone != null) bw.Write(client.NumeroTelephone);
        }
    }
}
