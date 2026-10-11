// src/models/TicketModel.ts
export interface ZoneOption {
  id: string;           // ZoneDayId
  zoneId: string;
  zoneName: string;
  raceDayId: string;
  capacity: number;
  seatsLeft: number;
  priceAddon: number;
}

export interface TicketModel {
  ticketId: string;      // backend vraća ticketId, ne id
  email: string;
  eventName: string;
  raceDay: string;
  zoneName: string;
  quantity: number;
  totalPrice: number;
  currencyCode: string;
  status: string;
  accessCode: string;
  availableZones: ZoneOption[];  // backend vraća listu dostupnih zona
  currentZoneDayId?: string;     // <--- DODATO
}