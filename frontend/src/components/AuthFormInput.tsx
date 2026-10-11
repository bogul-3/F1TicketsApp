type Props = {
    label: string;
    type?: string;
    value: string;
    placeholder?: string;
    onChange: (value: string) => void;
}

export default function AuthFormInput({ label, type = "text", value, onChange }: Props) {
  return (
    <div className="flex flex-col gap-1">
      <label className="text-sm font-medium">{label}</label>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="border rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-red-500"
      />
    </div>
  )
}