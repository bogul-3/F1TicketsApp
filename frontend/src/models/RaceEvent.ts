export interface RaceEvent {
  id: string
  name: string
  startDate: string
  endDate: string
  additionalInfo: string
  discountValidUntil: string
  cityId: string
}

export interface CreateRaceEventRequest {
  name: string
  startDate: string
  endDate: string
  additionalInfo: string
  discountValidUntil: string
  cityId: string
}

export interface ZoneDayHome {
  zoneDayId: string;
  capacity: number;
  seatsLeft: number;
  priceAddon: number;
}

export interface RaceDayHome {
  id: string;
  date: string;
  basePrice: number;
  zoneDays: ZoneDayHome[];
}

export interface RaceEventHome {
  id: string;
  name: string;
  city: string;
  startDate: string;
  endDate: string;
  raceDays: RaceDayHome[];
}