import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import AuthFormInput from "../components/AuthFormInput"
import type { Country, City } from "../models/Location"
import { getAllCountries, getCitiesByCountry } from "../services/locationService"
import { registerUser } from "../services/authService"
import { useAuth } from "../context/AuthContext"
import { saveToken } from "../utils/auth"

export default function RegisterPage() {
  const [name, setName] = useState("")
  const [surname, setSurname] = useState("")
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [address, setAddress] = useState("")
  const [postalCode, setPostalCode] = useState("")

  const [countries, setCountries] = useState<Country[]>([])
  const [cities, setCities] = useState<City[]>([])

  const [selectedCountry, setSelectedCountry] = useState<string>("")
  const [selectedCity, setSelectedCity] = useState<string>("")

  const [isLoading, setIsLoading] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")

  const navigate = useNavigate()
  const { login } = useAuth()

  // Type guard za role
  function isValidRole(role: string): role is "ADMIN" | "USER" {
    return role === "ADMIN" || role === "USER"
  }

  // fetch countries na mount
  useEffect(() => {
    getAllCountries()
      .then(setCountries)
      .catch(err => console.error(err))
  }, [])

  // fetch cities kad se promeni zemlja
  useEffect(() => {
    if (!selectedCountry) {
      setCities([])
      setSelectedCity("")
      return
    }
    getCitiesByCountry(selectedCountry)
      .then(setCities)
      .catch(err => console.error(err))
  }, [selectedCountry])

  const handleRegister = async () => {
    if (!name || !surname || !email || !password || !address || !postalCode || !selectedCity) {
      setErrorMessage("Molimo popunite sva polja")
      return
    }

    setIsLoading(true)
    setErrorMessage("")

    try {
      const response = await registerUser({
        name,
        surname,
        email,
        password,
        address,
        postalCode,
        cityId: selectedCity,
      })

      // Obaveštenje i redirect
      alert(`Uspešno ste se registrovali Dobrodošli, ${response.name}`)
      navigate("/")
    } catch (err: any) {
      setErrorMessage(err.message || "Registracija nije uspela")
      console.error("Registration failed:", err)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded-xl shadow-lg w-[400px] flex flex-col gap-4">
        <h1 className="text-2xl font-bold text-center">Register</h1>

        {errorMessage && <p className="text-red-500 text-center text-sm">{errorMessage}</p>}

        <AuthFormInput label="Name" value={name} onChange={setName} />
        <AuthFormInput label="Surname" value={surname} onChange={setSurname} />
        <AuthFormInput label="Email" value={email} onChange={setEmail} />
        <AuthFormInput label="Password" type="password" value={password} onChange={setPassword} />
        <AuthFormInput label="Address" value={address} onChange={setAddress} />
        <AuthFormInput label="Postal Code" value={postalCode} onChange={setPostalCode} />

        {/* Country */}
        <div className="flex flex-col">
          <label className="text-sm font-semibold mb-1">Country</label>
          <select
            className="border p-2 rounded"
            value={selectedCountry}
            onChange={(e) => setSelectedCountry(e.target.value)}
          >
            <option value="">Select country</option>
            {countries.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>

        {/* City */}
        <div className="flex flex-col">
          <label className="text-sm font-semibold mb-1">City</label>
          <select
            className="border p-2 rounded"
            value={selectedCity}
            onChange={(e) => setSelectedCity(e.target.value)}
            disabled={!selectedCountry || cities.length === 0}
          >
            <option value="">Select city</option>
            {cities.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>

        <button
          onClick={handleRegister}
          disabled={isLoading}
          className={`bg-red-600 text-white p-2 rounded-lg hover:bg-red-700 transition transform font-semibold ${
            isLoading ? "opacity-70 cursor-not-allowed" : ""
          }`}
        >
          {isLoading ? "Registering..." : "Register"}
        </button>
      </div>
    </div>
  )
}