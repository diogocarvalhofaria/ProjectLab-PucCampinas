import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { UserService } from '../../services/user.service';
import { UserResponse } from '../../models/user.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
})
export class UserFormComponent implements OnChanges {
  @Output() saved = new EventEmitter<void>();
  @Input() userData: UserResponse | null = null;

  form: FormGroup;

  constructor(private fb: FormBuilder, private userService: UserService) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      role: ['Student', Validators.required],
      phoneNumber: ['', Validators.required],

      cep: ['', Validators.required],
      logradouro: ['', Validators.required],
      bairro: ['', Validators.required],
      cidade: ['', Validators.required],
      estado: ['', Validators.required],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.userData) {
      this.form.patchValue(this.userData);
    } else {
      this.form.reset({ role: 'Student' });
    }
  }

  buscarCep() {
    const cepRaw = this.form.get('cep')?.value;

    if (!cepRaw) return;

    const cep = cepRaw.replace(/\D/g, '');

    if (cep.length === 8) {
      this.userService.consultarCep(cep).subscribe({
        next: (dados) => {
          this.form.patchValue({
            logradouro: dados.logradouro,
            bairro: dados.bairro,
            cidade: dados.localidade,
            estado: dados.uf,
          });
        },
        error: (err) => {
          console.error('Erro ao buscar CEP', err);
        },
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const formData = this.form.value;
      if (this.userData) {
        this.userService.update(this.userData.id, formData).subscribe({
          next: () => {
            alert('Usuário atualizado com sucesso!');
            this.saved.emit();
          },
          error: (err: HttpErrorResponse) => {
            console.error('Erro ao atualizar:', err);
            alert('Erro ao atualizar: ' + err.message);
          },
        });
      } else {
        this.userService.create(formData).subscribe({
          next: () => {
            alert('Usuário criado com sucesso!');
            this.saved.emit();
          },
          error: (err: HttpErrorResponse) => {
            console.error('Erro ao criar:', err);
            alert('Erro ao criar usuário.');
          },
        });
      }
    } else {
      this.form.markAllAsTouched();
    }
  }
}
