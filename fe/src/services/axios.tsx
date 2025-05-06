import axios, { AxiosError, AxiosRequestConfig, AxiosResponseTransformer } from "axios";

let accessToken: string | null = null;

const isoDateRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d+$/;

const parseDates = (obj: any): any => {
    if (obj === null || obj === undefined) return obj;

    if (typeof obj === 'string' && isoDateRegex.test(obj)) {
        return new Date(obj);
    }

    if (Array.isArray(obj)) {
        return obj.map(parseDates);
    }

    if (typeof obj === 'object') {
        const result: any = {};
        for (const key in obj) {
            result[key] = parseDates(obj[key]);
        }
        return result;
    }

    return obj;
};

const axiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    transformResponse: [
        ...(axios.defaults.transformResponse as AxiosResponseTransformer[]),
        (data) => parseDates(data)
    ]
});

axios.defaults.transformResponse

axiosInstance.interceptors.request.use(
    (config) => {
        config.withCredentials = true;

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

let isRefreshing = false;

let failedQueue: {
    resolve: (token: string) => void;
    reject: (err: any) => void;
}[] = [];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach((prom) => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token!);
        }
    });
    failedQueue = [];
};

const refreshAccessToken = async (): Promise<string> => {
    const accessToken = await axios.get<string>(`${import.meta.env.VITE_API_URL}/auth/refresh`, { withCredentials: true });
    return accessToken.data;
}

const nonRetryUrls = [
    "/auth/login",
    "/auth/logout"
];

axiosInstance.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

        if (error.response?.status === 401 && !originalRequest._retry && !nonRetryUrls.includes(error.config?.url || "")) {
            originalRequest._retry = true;

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then((token) => {
                    originalRequest.headers = {
                        ...originalRequest.headers,
                        Authorization: `Bearer ${token}`,
                    };
                    return axiosInstance(originalRequest);
                });
            }

            isRefreshing = true;

            try {
                const newToken = await refreshAccessToken();
                accessToken = newToken;
                processQueue(null, newToken);
                originalRequest.headers = {
                    ...originalRequest.headers,
                    Authorization: `Bearer ${newToken}`,
                };
                return axiosInstance(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError, null);

                if (window.location.pathname !== "/login" && window.location.pathname !== "/register") {
                    window.location.pathname = "/login";
                }

                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
)

const setAccessToken = (token: string) => accessToken = token;

const clearAccessToken = () => accessToken = null;

export {
    axiosInstance,
    setAccessToken,
    clearAccessToken
}