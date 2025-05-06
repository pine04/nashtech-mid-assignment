import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner";
import { AxiosError } from "axios";
import { rejectRequest } from "../services/requestsServices";

export const useRejectRequest = (requestId: number) => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => rejectRequest(requestId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("requests")
            });
            toast.success(`Rejected request with ID ${requestId}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while rejecting request with ID ${requestId}.`);
        }
    });

    return {
        rejectRequest: mutation.mutateAsync
    }
}