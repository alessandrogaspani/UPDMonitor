# Manutenzione

## Linee guida

- preferire test unitari sui ViewModel quando possibile
- ridurre la dipendenza da timing e rete nei test
- mantenere separati UI, business e infrastruttura
- riusare i servizi condivisi in `UDPMonitor.Core`

## Evoluzioni consigliate

- introdurre più test di comportamento sui ViewModel
- migliorare l'isolamento dei test di integrazione UDP
- aggiungere documentazione per i nuovi comandi e dialoghi

## Wiki e documentazione

Quando aggiungi nuove feature, aggiorna sempre:

- overview
- testing
- configurazione
- eventuali note di rilascio
