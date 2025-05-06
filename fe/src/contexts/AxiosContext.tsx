// import axios, { AxiosInstance } from "axios";
// import { createContext, ReactNode } from "react";
// import { useAuth } from "./AuthContext";

// interface AxiosContextType {
//     axios: AxiosInstance;
// }

// const AxiosContext = createContext<AxiosContextType | null>(null);

// const AxiosContextProvider = ({ children }: { children: ReactNode }) => {
//     const { accessToken, updateAccessToken, clearAccessToken } = useAuth();

//     const axiosInstance = axios.create({
//         baseURL: import.meta.env.VITE_API_URL,
//     });

//     axiosInstance.interceptors.request.use(
//         (config) => {
//             if (accessToken) {
//                 config.headers.Authorization = `Bearer ${accessToken}`;
//             }
//             return config;
//         },
//         (error) => Promise.reject(error)
//     );

//     axiosInstance.interceptors.response.use(
//         (response) => response,
//         async (error) => {
//             const originalRequest = error.config;

//             if (error.response?.status === 401 && !originalRequest._retry) {
//                 if (isRefreshing) {
//                     return new Promise((resolve, reject) => {
//                         failedQueue.push({ resolve, reject });
//                     })
//                         .then((token) => {
//                             originalRequest.headers.Authorization = "Bearer " + token;
//                             return api(originalRequest);
//                         })
//                         .catch((err) => Promise.reject(err));
//                 }

//                 originalRequest._retry = true;
//                 isRefreshing = true;

//                 try {
//                     const newToken = await refreshAccessToken(); // calls /refresh
//                     processQueue(null, newToken);
//                     originalRequest.headers.Authorization = "Bearer " + newToken;
//                     return api(originalRequest);
//                 } catch (err) {
//                     processQueue(err, null);
//                     return Promise.reject(err);
//                 } finally {
//                     isRefreshing = false;
//                 }
//             }

//             return Promise.reject(error);
//         }
//     );
// }