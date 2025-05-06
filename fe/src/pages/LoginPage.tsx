import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../components/ui/form";
import { Input } from "../components/ui/input";
import { Button } from "../components/ui/button";
import { Link } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { AxiosError } from "axios";
import { toast } from "sonner";

const formSchema = z.object({
    email: z.string().nonempty().email(),
    password: z.string().nonempty()
});

export const LoginPage = () => {
    const { loginMutation } = useAuth();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            email: "",
            password: ""
        },
    })

    async function onSubmit(values: z.infer<typeof formSchema>) {
        try {
            await loginMutation.mutateAsync(values);
        } catch (error: any) {
            if (error instanceof AxiosError) {
                toast.error(error.response?.data.detail);
            }
            console.log(error);
        }
    }

    return (
        <div className="h-full flex items-center justify-center px-8">
            <div>

                <p className="text-4xl font-bold text-center mb-8">Library Management</p>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="w-full max-w-sm border border-slate-300 rounded-lg mx-auto p-8 flex flex-col gap-4">
                        <h1 className="text-center font-bold text-2xl">Login</h1>

                        <FormField
                            control={form.control}
                            name="email"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Email</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <FormField
                            control={form.control}
                            name="password"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Password</FormLabel>
                                    <FormControl>
                                        <Input type="password" {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <Button type="submit" className="bg-blue-800 w-fit self-center hover:bg-blue-600" disabled={loginMutation.isPending}>
                            Log in
                        </Button>

                        <p className="text-center">Don't have an account? <Link to="/register" className="underline text-blue-800">Register</Link> instead.</p>
                    </form>
                </Form>
            </div>
        </div>
    )
}