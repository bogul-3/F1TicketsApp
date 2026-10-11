export interface Zone {
  id: string
  name: string
}

export interface ZoneModel {
  id: string;
  zoneDayid: string;
  name: string;
  capacity: number;
  seatsLeft: number;
  priceAddon: number;
  isSelected: boolean;
}