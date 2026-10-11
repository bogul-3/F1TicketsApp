import type { ZoneModel } from "../models/Zone.js";
import { type FC } from "react";
import ZoneListRow from "./ZoneListRow.js";

interface Props {
  zones: ZoneModel[];
  onZoneClick: (id: string) => void;
}

const ZoneList: FC<Props> = ({ zones, onZoneClick }) => {
  return (
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Capacity</th>
          <th>Seats Left</th>
          <th>Price Addon</th>
        </tr>
      </thead>
      <tbody>
        {zones.map((zone) => (
          <ZoneListRow
            zone={zone}
            key={zone.id}
            onClick={() => onZoneClick(zone.id)}
          ></ZoneListRow>
        ))}
      </tbody>
    </table>
  );
};

export default ZoneList;
