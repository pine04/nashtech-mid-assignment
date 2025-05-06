import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from "react-router-dom";
import { CartContextProvider } from '@/contexts/CartContext';
import { AppRoutes } from '@/routes/AppRoutes';
import { AuthContextProvider } from './contexts/AuthContext';

const queryClient = new QueryClient();

function App() {
    return (
        <BrowserRouter>
            <QueryClientProvider client={queryClient}>
                <AuthContextProvider>
                    <CartContextProvider>
                        <AppRoutes />
                    </CartContextProvider>
                </AuthContextProvider>
            </QueryClientProvider>
        </BrowserRouter>
    )
}

export default App
