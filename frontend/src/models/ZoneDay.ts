export interface ZoneDay {
  id: string;
  seatsLeft: number;
  capacity: number;
  priceAddon: number;
  raceDayId: string;
  zoneId: string;
  zoneName?: string; // za prikaz u tabeli
}