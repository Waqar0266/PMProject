import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7161/api"
});

export default api;