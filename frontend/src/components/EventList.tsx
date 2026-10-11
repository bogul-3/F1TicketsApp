import { type FC } from "react";
import type { EventModel } from "../models/Event.js";
import EventListRow from "./EventListRow.js";

interface Props {
  events: EventModel[];
  onEventClick: (id: string) => void;
}

const EventList: FC<Props> = ({ events, onEventClick }) => {
  return (
    <div className="overflow-x-auto shadow-xl rounded-lg m-6 border border-gray-200">
    <table className="w-full text-sm text-left">
      <thead className="text-xs text-white uppercase bg-black">
        <tr>
          <th className="px-6 py-4">Id</th>
          <th className="px-6 py-4">Name</th>
          <th className="px-6 py-4">Location</th>
          <th className="px-6 py-4">Date</th>
          <th className="px-6 py-4">Additional information</th>
        </tr>
      </thead>
      <tbody className="divide-y divide-gray-100 bg-white">
        {events.map((evnt) => (
          <EventListRow evnt={evnt} key={evnt.id} onClick={() => onEventClick(evnt.id ?? "")}></EventListRow>
        ))}
      </tbody>
    </table>
    </div>
  );
};

export default EventList;
