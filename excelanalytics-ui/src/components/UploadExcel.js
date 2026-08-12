import { useState } from "react";
import api from "../services/api";

export default function UploadExcel({ onUploadSuccess }) {

    const [file, setFile] = useState(null);
    const [uploadResult, setUploadResult] = useState(null);

    const upload = async () => {

        if (!file) {
            alert("Please select an Excel file.");
            return;
        }

        const formData = new FormData();
        formData.append("file", file);

        try {

            const response = await api.post("/Upload", formData, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            });

            setUploadResult(response.data);

            if (onUploadSuccess)
                onUploadSuccess();

        }
        catch (err) {
            console.log(err);
            setUploadResult(null);
            alert(err.response?.data || "Upload failed.");
        }
    };

    return (
        <div>
            <div className="d-flex gap-2 align-items-start">
                <input
                    type="file"
                    className="form-control"
                    style={{ maxWidth: "280px" }}
                    accept=".xlsx,.xls"
                    onChange={(e) => {
                        setFile(e.target.files[0]);
                        setUploadResult(null);
                    }}
                />

                <button
                    className="btn btn-primary"
                    onClick={upload}>
                    Upload Excel
                </button>
            </div>

            {uploadResult && (
                <div className="mt-3" style={{ maxWidth: "520px" }}>
                    <div className={`alert ${uploadResult.missingRateWarnings?.length ? "alert-warning" : "alert-success"} mb-2`}>
                        <strong>{uploadResult.message}</strong>
                        <div className="mt-1">
                            {uploadResult.recordsImported} record(s) imported.
                            {uploadResult.recordsWithMissingRate > 0 && (
                                <span>
                                    {" "}{uploadResult.recordsWithMissingRate} row(s) have no matching rate.
                                </span>
                            )}
                        </div>
                    </div>

                    {uploadResult.missingRateWarnings?.map((warning, index) => (
                        <div key={index} className="alert alert-danger py-2 mb-2">
                            <strong>Missing rate:</strong>{" "}
                            Country <code>{warning.country}</code>, Project <code>{warning.project}</code>,{" "}
                            {warning.monthName} {warning.year}
                            <div className="mt-1 small">
                                {warning.rowCount} affected row(s)
                                {warning.employees?.length > 0 && (
                                    <ul className="mb-0 mt-1 ps-3">
                                        {warning.employees.map((emp, i) => (
                                            <li key={i}>{emp}</li>
                                        ))}
                                        {warning.rowCount > warning.employees.length && (
                                            <li>...and {warning.rowCount - warning.employees.length} more</li>
                                        )}
                                    </ul>
                                )}
                            </div>
                            <div className="small mt-1 text-muted">
                                Configure this in Rate Configuration, then re-upload or update the rate to recalculate.
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
