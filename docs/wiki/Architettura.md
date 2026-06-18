# Architettura

UDPMonitor è organizzato in più progetti:

## UDPMonitor

Contiene la UI WPF, i ViewModel e le views.

Componenti principali:

- `ViewModels`
- `Views`
- `Behaviors`
- `Converters`
- `Styles`
- `App.xaml` e bootstrap Prism

## UDPMonitor.Business

Contiene la logica di business e i servizi applicativi.

## UDPMonitor.Core

Contiene servizi infrastrutturali e componenti di base, tra cui:

- `ApplicationService`
- gestione configurazione
- canali UDP
- utility condivise

## UDPMonitor.Tests

Contiene test unitari e di integrazione.

## Pattern usati

- MVVM
- Prism
- dependency injection
- command binding
- observable collections per la UI

## Navigazione

La shell principale registra e naviga nelle view tramite regioni Prism.
