import { useEffect, useState } from "react";
import api from "../services/api";

export default function RateConfiguration() {

    const [countries, setCountries] = useState([]);
    const [rates, setRates] = useState([]);
const [currencies, setCurrencies] = useState([]);
const [currencyId, setCurrencyId] = useState("");

    const [countryId, setCountryId] = useState("");
    const [year, setYear] = useState(new Date().getFullYear());
    const [month, setMonth] = useState(new Date().getMonth() + 1);
    const [rate, setRate] = useState("");
    const [search, setSearch] = useState("");
const [editingId, setEditingId] = useState(null);
    useEffect(() => {

    loadCountries();
loadCurrencies();
loadRates();

    }, []);

    const loadCountries = () => {

        api.get("/Countries")
            .then(res => setCountries(res.data))
            .catch(err => console.log(err));

    };
    const loadCurrencies = () => {

    api.get("/Currency")
        .then(res => setCurrencies(res.data))
        .catch(err => console.log(err));

};

    const loadRates = () => {

    api.get("/RateConfigurations")
        .then(res => {
            console.log("Rates API:", res.data);
            setRates(res.data);
        })
        .catch(err => console.log(err));

};
    const saveRate = () => {

    if (countryId === "") {

        alert("Please select country.");

        return;

    }
    if (currencyId === "") {

    alert("Please select currency.");

    return;

}

    if (rate === "") {

        alert("Please enter rate.");

        return;

    }

api.post("/RateConfigurations", {

    countryId,
    currencyId,
    year: Number(year),
    month: Number(month),
    rate: Number(rate)

})

    .then(res => {

    alert("Rate saved successfully.");

    setCountryId("");
    setCurrencyId("");
    setRate("");
    setYear(new Date().getFullYear());
    setMonth(new Date().getMonth() + 1);

    loadRates();

})

    .catch(err => {

        console.log(err);

        alert("Unable to save rate.");

    });

};
const editRate = (item) => {

   setEditingId(item.id);

setCountryId(item.countryId);

setCurrencyId(item.currencyId);

setYear(item.year);

setMonth(item.month);

setRate(item.rate);

};
const updateRate = () => {

  api.put(`/RateConfigurations/${editingId}`, {

    countryId,
    currencyId,
    year: Number(year),
    month: Number(month),
    rate: Number(rate)

})

   .then(() => {

    alert("Rate updated successfully.");

    setEditingId(null);

    setCountryId("");
    setCurrencyId("");
    setRate("");

    setYear(new Date().getFullYear());
    setMonth(new Date().getMonth() + 1);

    loadRates();

})

    .catch(err => {

        console.log(err);

        alert("Update failed.");

    });

};
  const filteredRates = rates.filter(x => {

        return (
            x.countryName.toLowerCase().includes(search.toLowerCase()) ||
            x.currencyCode.toLowerCase().includes(search.toLowerCase()) ||
            x.currencyName.toLowerCase().includes(search.toLowerCase())
        );

    });
const deleteRate = (id) => {

    if (!window.confirm("Delete this rate?"))
        return;

    api.delete(`/RateConfigurations/${id}`)

    .then(() => {

        alert("Deleted successfully.");

        loadRates();

    })

    .catch(err => {

        console.log(err);

        alert("Delete failed.");

    });

};

    return (

        <div className="container-fluid p-4">

            <div className="card shadow border-0">

                <div className="card-header bg-white">

                    <h3 className="mb-0">

                        💰 Rate Configuration

                    </h3>

                </div>

                <div className="card-body">

                  <div className="row g-3">

    {/* Country */}
    <div className="col-md-3">
        <label className="form-label fw-semibold">
            Country
        </label>

        <select
            className="form-select"
            value={countryId}
            onChange={(e) => setCountryId(e.target.value)}
        >
            <option value="">
                Select Country
            </option>

            {countries.map(country => (
                <option
                    key={country.id}
                    value={country.id}
                >
                    {country.name}
                </option>
            ))}
        </select>
    </div>

    {/* Currency */}
    <div className="col-md-3">
        <label className="form-label fw-semibold">
            Currency
        </label>

        <select
            className="form-select"
            value={currencyId}
            onChange={(e) => setCurrencyId(e.target.value)}
        >
            <option value="">
                Select Currency
            </option>

            {currencies.map(currency => (
                <option
                    key={currency.id}
                    value={currency.id}
                >
                    {currency.code} ({currency.symbol})
                </option>
            ))}
        </select>
    </div>

    {/* Year */}
    <div className="col-md-2">
        <label className="form-label fw-semibold">
            Year
        </label>

        <input
            type="number"
            className="form-control"
            value={year}
            onChange={(e) => setYear(e.target.value)}
        />
    </div>

    {/* Month */}
    <div className="col-md-2">
        <label className="form-label fw-semibold">
            Month
        </label>

        <select
            className="form-select"
            value={month}
            onChange={(e) => setMonth(e.target.value)}
        >
            <option value="1">January</option>
            <option value="2">February</option>
            <option value="3">March</option>
            <option value="4">April</option>
            <option value="5">May</option>
            <option value="6">June</option>
            <option value="7">July</option>
            <option value="8">August</option>
            <option value="9">September</option>
            <option value="10">October</option>
            <option value="11">November</option>
            <option value="12">December</option>
        </select>
    </div>

    {/* Rate */}
    <div className="col-md-2">
        <label className="form-label fw-semibold">
            Rate
        </label>

        <input
            type="number"
            className="form-control"
            placeholder="Enter Rate"
            value={rate}
            onChange={(e) => setRate(e.target.value)}
        />
    </div>

</div>

<div className="row mt-4">

    <div className="col-12 text-end">

        <button
            className={`btn ${
                editingId ? "btn-warning" : "btn-success"
            } px-5`}
            onClick={editingId ? updateRate : saveRate}
        >
            {editingId ? "Update Rate" : "Save Rate"}
        </button>

    </div>

</div>
<div className="row mb-3">

    <div className="col-md-4">

        <input
            className="form-control"
            placeholder="🔍 Search Country or Currency..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
        />

    </div>

    <div className="col-md-8 text-end">

        <span className="badge bg-primary fs-6">

            {filteredRates.length} Records

        </span>

    </div>

</div>
                    <hr />

                    <table className="table table-hover">

                        <thead className="table-dark">

                            <tr>

                                <th>Country</th>
<th>Currency</th>
<th>Year</th>
<th>Month</th>
<th>Rate</th>
<th>Actions</th>

                            </tr>

                        </thead>

                        <tbody>

                           {filteredRates.map(rate =>  (

                                <tr key={rate.id}>

                                    <td>{rate.countryName}</td>

                                 <td>
    {rate.currencySymbol} {rate.currencyCode}
</td>

<td>{rate.year}</td>

                                 <td>
{
[
"",
"January",
"February",
"March",
"April",
"May",
"June",
"July",
"August",
"September",
"October",
"November",
"December"
][rate.month]
}
</td>

                                    <td>{rate.rate}</td><td className="fw-bold text-success">

    {rate.currencySymbol} {Number(rate.rate).toLocaleString()}

</td>
                                    <td>
<button
    className="btn btn-warning btn-sm me-2"
    onClick={() => editRate(rate)}
>
    Edit
</button>

                                      <button
    className="btn btn-danger btn-sm"
    onClick={() => deleteRate(rate.id)}
>
    Delete
</button>

                                    </td>

                                </tr>

                            ))}

                        </tbody>

                    </table>

                </div>

            </div>

        </div>

    );

}