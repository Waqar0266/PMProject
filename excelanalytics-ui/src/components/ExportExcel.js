import * as XLSX from "xlsx";
import { saveAs } from "file-saver";

export default function ExportExcel({ data }) {

    const exportToExcel = () => {

        const worksheet = XLSX.utils.json_to_sheet(data);

        const workbook = XLSX.utils.book_new();

        XLSX.utils.book_append_sheet(
            workbook,
            worksheet,
            "Employees"
        );

        const excelBuffer =
            XLSX.write(workbook, {
                bookType: "xlsx",
                type: "array"
            });

        const file = new Blob(
            [excelBuffer],
            {
                type:
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            });

        saveAs(file, "EmployeeReport.xlsx");
    };

    return (

        <button
            className="btn btn-success"
            onClick={exportToExcel}
        >
            📥 Export Excel
        </button>

    );

}