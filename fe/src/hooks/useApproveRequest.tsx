import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner";
import { AxiosError } from "axios";
import { approveRequest } from "../services/requestsServices";

export const useApproveRequest = (requestId: number) => {
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => approveRequest(requestId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("requests")
            });
            toast.success(`Approved request with ID ${requestId}.`);
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || `An error happened while approving request with ID ${requestId}.`);
        }
    });

    return {
        approveRequest: mutation.mutateAsync
    }
}