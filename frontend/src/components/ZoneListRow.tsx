import { type FC } from "react";
import type { ZoneModel } from "../models/Zone.js";

interface Props {
  zone: ZoneModel;
  onClick?: () => void;
}

const ZoneListRow: FC<Props> = ({ zone, onClick }) => {
  return (
    <tr
      key={zone.id}
      onClick={onClick}
      className={` ${zone.isSelected ? "bg-green-500" : "bg-yellow-500"}`}
    >
      <td>{zone.name}</td>
      <td>
        {zone.seatsLeft}/{zone.capacity}
      </td>
      <td>{zone.priceAddon}</td>
    </tr>
  );
};

export default ZoneListRow;
