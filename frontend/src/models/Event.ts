export interface EventModel {
  id?: string;
  name?: string;
  city?: CityModel;
  startDate?: string;
  endDate?: string;
  additionalInfo?: string;
}

export interface CityModel {
  name?: string;
}
