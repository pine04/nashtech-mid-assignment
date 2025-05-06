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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../components/ui/select";

const formSchema = z.object({
    firstName: z.string().nonempty(),
    lastName: z.string().nonempty(),
    email: z.string().nonempty().email(),
    password: z.string().nonempty(),
    role: z.union([z.literal(0), z.literal(1)])
});

export const RegisterPage = () => {
    const { registerMutation } = useAuth();

    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            firstName: "",
            lastName: "",
            email: "",
            password: "",
            role: 0
        },
    })

    async function onSubmit(values: z.infer<typeof formSchema>) {
        try {
            await registerMutation.mutateAsync(values);
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
                        <h1 className="text-center font-bold text-2xl">Register</h1>

                        <FormField
                            control={form.control}
                            name="firstName"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>First name</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <FormField
                            control={form.control}
                            name="lastName"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Last name</FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

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

                        <FormField
                            control={form.control}
                            name="role"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Role</FormLabel>
                                    <FormControl>
                                        <Select {...field} value={field.value.toString()} onValueChange={(value) => form.setValue("role", value === "0" ? 0 : 1)}>
                                            <SelectTrigger className="w-full">
                                                <SelectValue placeholder="Select a role" />
                                            </SelectTrigger>
                                            <SelectContent>
                                                <SelectItem value="0">Normal user</SelectItem>
                                                <SelectItem value="1">Super user</SelectItem>
                                            </SelectContent>
                                        </Select>
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <Button type="submit" className="bg-blue-800 w-fit self-center hover:bg-blue-600" disabled={registerMutation.isPending}>
                            Register
                        </Button>

                        <p className="text-center">Already have an account? <Link to="/login" className="underline text-blue-800">Login</Link> instead.</p>
                    </form>
                </Form>
            </div>
        </div>
    )
}