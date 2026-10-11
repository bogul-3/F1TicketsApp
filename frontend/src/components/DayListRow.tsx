import { type FC } from "react";
import type { DayModel } from "../models/Day.js";

interface Props {
  day: DayModel;
  onClick?: () => void
}

const DayListRow: FC<Props> = ({ day, onClick }) => {
  const formatDate = (date: Date) => {
    if (!date) {
      return "N/A";
    }
    const formatedDate = `${date.getDay()}.${date.getMonth()}.${date.getFullYear()}`;
    return formatedDate;
  };

  return (
    <tr
      key={day.id}
      onClick={onClick}
      className={`
        border-b transition-colors cursor-pointer
        ${
          day.isSelected
            ? "bg-green-100 border-green-200 hover:bg-green-200"
            : "bg-white hover:bg-gray-50 text-gray-700"
        }
      `}
    >
      <td className="px-6 py-4 font-medium italic">{formatDate(day.date)}</td>
      <td className="px-6 py-4 font-semibold">{day.basePrice}</td>
    </tr>
  );
};

export default DayListRow;
