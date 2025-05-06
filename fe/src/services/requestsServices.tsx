import { Allowance, PagedResponse, Request } from "../types/types";
import { axiosInstance } from "./axios";

export const getMyAllowance = async (): Promise<Allowance> => {
    const response = await axiosInstance.get<Allowance>("/my-allowance");
    return response.data;
}

export const getMyBorrowings = async (pageNumber: number = 1, pageSize: number = 10): Promise<PagedResponse<Request>> => {
    const response = await axiosInstance.get<PagedResponse<Request>>("/my-requests", {
        params: {
            pageNumber,
            pageSize
        }
    });

    return response.data;
}

interface MakeBorrowingRequestRequest {
    bookIds: number[];
}

export const makeBorrowingRequest = async (request: MakeBorrowingRequestRequest): Promise<Request> => {
    const response = await axiosInstance.post<Request>("/my-requests", request);
    return response.data;
}

export const getRequests = async (pageNumber: number = 1, pageSize: number = 10): Promise<PagedResponse<Request>> => {
    const response = await axiosInstance.get<PagedResponse<Request>>("/requests", {
        params: {
            pageNumber,
            pageSize
        }
    });

    return response.data;
}

export const approveRequest = async (requestId: number): Promise<void> => {
    await axiosInstance.post(`/requests/${requestId}/approve`);
}

export const rejectRequest = async (requestId: number): Promise<void> => {
    await axiosInstance.post(`/requests/${requestId}/reject`);
}