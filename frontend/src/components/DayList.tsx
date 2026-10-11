import { type FC } from "react";
import DayListRow from "./DayListRow.js";
import type { DayModel } from "../models/Day.js";

interface Props {
  days: DayModel[];
  onDayClick: (id: string) => void;
}

const DayList: FC<Props> = ({ days, onDayClick }) => {
  return (
    <div className="overflow-x-auto shadow-md sm:rounded-lg m-4">
    <table className="w-full text-sm text-left text-gray-500">
      <thead className="text-xs text-gray-700 uppercase bg-gray-100">
        <tr>
          <th className="px-6 py-3">Date</th>
          <th className="px-6 py-3">Price</th>
        </tr>
      </thead>
      <tbody>
        {days.map((day) => (
          <DayListRow day={day} key={day.id} onClick={() => onDayClick(day.id)}></DayListRow>
        ))}
      </tbody>
    </table>
    </div>
  );
};

export default DayList; 
