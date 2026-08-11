import { useState } from "react";
import api from "../services/api";

export default function UploadExcel({ onUploadSuccess }) {

    const [file, setFile] = useState(null);

    const upload = async () => {

        if (!file) {
            alert("Please select an Excel file.");
            return;
        }

        const formData = new FormData();
        formData.append("file", file);

        try {

            await api.post("/Upload", formData, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            });

            alert("Excel uploaded successfully.");

            if (onUploadSuccess)
                onUploadSuccess();

        }
        catch (err) {
            console.log(err);
            alert("Upload failed.");
        }
    };

    return (
        <div className="card mb-4">
            <div className="card-body">

                <input
                    type="file"
                    className="form-control mb-3"
                    accept=".xlsx,.xls"
                    onChange={(e) => setFile(e.target.files[0])}
                />

                <button
                    className="btn btn-primary"
                    onClick={upload}>
                    Upload Excel
                </button>

            </div>
        </div>
    );
}