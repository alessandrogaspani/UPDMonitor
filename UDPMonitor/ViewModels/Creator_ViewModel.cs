using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using UDPMonitor.ViewModels.Base;

namespace UDPMonitor.ViewModels
{
    public class Creator_ViewModel : DialogViewModelBase
    {
        private readonly Random _rnd = new Random();
        private int _index;

        public ObservableCollection<string> Quotes { get; } = new ObservableCollection<string>();

        private string _currentQuote;

        public string CurrentQuote
        {
            get => _currentQuote;
            set => SetProperty(ref _currentQuote, value);
        }

        public DelegateCommand NextQuoteCommand { get; }
        public DelegateCommand RandomQuoteCommand { get; }

        public Creator_ViewModel()
        {
            Title = "👀 Easter Egg";

            NextQuoteCommand = new DelegateCommand(NextQuote);
            RandomQuoteCommand = new DelegateCommand(RandomQuote);

            // Qui dentro inserisci le citazioni
            FillQuotes();
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            RandomQuote();
        }

        // ✅ Metodo “unico” dove inserisci le citazioni
        private void FillQuotes()
        {
            Quotes.Clear();

            Quotes.Add("Scacco matto!");
            Quotes.Add("Le mie aperture preferite? Da bianco il Gambetto di donna, da nero la Siciliana!");
            Quotes.Add("Teo, E' questo il giorno?");
            Quotes.Add("A che versione di D&D giochi?");
            Quotes.Add("Fac, sei il migliore!");
            Quotes.Add("Bravo Johnny!");
            Quotes.Add("Giovedi campetto?");
            Quotes.Add("Dove è il metro del TBMX?");
            Quotes.Add("La magia esiste e si chiama dependency injection.");
            Quotes.Add("Ciao John!");
            Quotes.Add("Scrrscrrr e josip, i miei F-Sensor preferiti.");
            Quotes.Add("..and then..Boom!");
            Quotes.Add("Non è tutto oro quel che lucida.");
            Quotes.Add("abilitagliglia");
            Quotes.Add("Manuel, ti cercano al service!");
            Quotes.Add("Quando avremo il TBPrism...");
            Quotes.Add("Hai risolto il bug dell'uomo nero?");
            Quotes.Add("Il TBMX è come l'inferno dantesco, o meglio i 9 inferi di Baator..in fondo c'e' Asmodeus");
            Quotes.Add("Chiediamolo al nostro ingegnere biomedico preferito!");
            Quotes.Add("Hai indossato la giubba?");
            Quotes.Add("Tanta roba il TBMX..");
            Quotes.Add("Per questo potresti vincere l'oscar del codice.");
            Quotes.Add("Hai mai sentito parlare dei principi SOLID?");
            Quotes.Add("Sei sicuro di volerne fare un NuGet?");
            Quotes.Add("La cosiddetta 'Gaspanata'");
            Quotes.Add("Leo, Non puoi indossare due scudi!");
            Quotes.Add("I githyanki cavalcano i draghi rossi come alleati d’acciaio: un patto antico, una guerra eterna, e il cielo che brucia quando volano insieme.");
            Quotes.Add("Controlla come stanno le vespe.. si muovono?");
            Quotes.Add("Stai usando il tool dell'UDP? 2 centesimi a me..");
            Quotes.Add("1. e4 c5 2. Nf3 e6 3. d4 cxd4 4. Nxd4 a6 5. Nc3 Qc7 6. Be2 Nf6 7. O-O Nc6 8. f4 Nxd4 9. Qxd4 Bc5 ...");
            Quotes.Add("E' giovedi, Manuel Savoy?");
            Quotes.Add("Mi sa che c'e' un bug..ah no, è giusto!");
            Quotes.Add("Il fallimento di una vita..");
            Quotes.Add("Come sei messo?");
            Quotes.Add("Ce la giochiamo sul parquet?");
            Quotes.Add("Simone è troppo forte a scacchi..");
            Quotes.Add("Il sistema di UDM è la mia parte preferita del TBMX...");
            Quotes.Add("La locanda del cane...");
            Quotes.Add("Al cheee!");
            Quotes.Add("Manuel, ma a te cosa piace?");
            Quotes.Add("Quando c'era il calcetto...bei tempi...");
            Quotes.Add("Bombetta?...");
            Quotes.Add("Buongiorno Signor Stefano!...Goooal");
            Quotes.Add("Nell'era di moreno...");
            Quotes.Add("Piede sinistro");
            Quotes.Add("Questa è la cosa più intelligente che ho sentito nell'ultimo mese");
            Quotes.Add("Molto molto molto molto bene...");
            Quotes.Add("E' una richiesta?...");
            Quotes.Add("Ma mai nella vita...");
            Quotes.Add("Stai attento al gambetto nepalese!");
            Quotes.Add("La Scandinava, ma anche il Gambetto Italiano.. solo sorprese.");
            Quotes.Add("Gina Pellicci, vero Andre?");
            Quotes.Add("Però!");
            Quotes.Add("Non metto in dubbio la tua velocità..");
            Quotes.Add("Fidaty Oro");
            Quotes.Add("They are taking the hobbits to Isengard");
            Quotes.Add("Appendino Philip?");
            Quotes.Add("Attento alle dipendenze di terze parti!");
            Quotes.Add("Gli esercizi dovrebbero avere l'avanzamento, come i mostri di D&D.");
            Quotes.Add("Andrei a lavorare nella Giacosi Srl");
            Quotes.Add("Manuel, ma a te cosa piace?");
            Quotes.Add("Tira fuori il cornetto della annunciazione..");
            Quotes.Add("I sub events, ovvero il pattern Esselunga");
            Quotes.Add("Quale è la password del DB? TecnoC...");
            Quotes.Add("Se lo faccio... Alessandro Gaspani, che non gioca...");
            Quotes.Add("Per cosa sta WPF?");
            Quotes.Add("Addio.");
        }

        // Se preferisci popolarle da fuori:
        public void SetQuotes(IEnumerable<string> quotes)
        {
            Quotes.Clear();
            foreach (var q in quotes)
                Quotes.Add(q);

            _index = 0;
            PickQuote(_index);
        }

        public void AddQuote(string quote)
        {
            if (string.IsNullOrWhiteSpace(quote))
                return;

            Quotes.Add(quote.Trim());

            if (string.IsNullOrWhiteSpace(CurrentQuote))
                PickQuote(Quotes.Count - 1);
        }

        private void NextQuote()
        {
            if (Quotes.Count == 0) return;

            _index++;
            if (_index >= Quotes.Count) _index = 0;
            PickQuote(_index);
        }

        private void RandomQuote()
        {
            if (Quotes.Count == 0) return;

            _index = _rnd.Next(0, Quotes.Count);
            PickQuote(_index);
        }

        private void PickQuote(int idx)
        {
            if (Quotes.Count == 0)
            {
                CurrentQuote = string.Empty;
                return;
            }

            if (idx < 0) idx = 0;
            if (idx >= Quotes.Count) idx = Quotes.Count - 1;

            CurrentQuote = Quotes[idx];
        }
    }
}