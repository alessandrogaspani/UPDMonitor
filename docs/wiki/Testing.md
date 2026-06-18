# Testing

## Struttura attuale

Il repository include un progetto test dedicato:

- `UDPMonitor.Tests`

## Tipi di test

- **Unit test**: verificano ViewModel e logica di comportamento isolata
- **Integration test**: verificano componenti UDP con socket reali

## Aree coperte

- `About_ViewModel`
- `Inbound_ViewModel`
- `Outbound_ViewModel`
- `UdpService`
- `UdpInChannel`
- `UdpOutChannel`

## Nota sui test UDP

I test che aprono socket reali possono essere più lenti e meno deterministici dei test puramente unitari.
Conviene tenerli pochi e focalizzati sul comportamento più importante.

## Esecuzione

I test possono essere eseguiti da Visual Studio Test Explorer o tramite build/test della soluzione.
