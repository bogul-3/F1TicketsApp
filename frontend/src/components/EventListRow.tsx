import { type FC } from "react";
import type { EventModel } from "../models/Event.js";

interface Props {
  evnt: EventModel;
  onClick?: () => void
}

const EventListRow: FC<Props> = ({ evnt, onClick}) => {
  return (
    <tr key={evnt.id} onClick={onClick}>
      <td>{evnt.id}</td>
      <td>{evnt.name}</td>
      <td>{evnt.city.name}</td>
      <td>
        {evnt.startDate} - {evnt.endDate}
      </td>
      <td>{evnt.additionalInfo}</td>
    </tr>
  );
};

export default EventListRow;
